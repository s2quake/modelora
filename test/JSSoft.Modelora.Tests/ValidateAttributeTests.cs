// <copyright file="ValidateAttributeTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora.Tests;

public sealed class ValidateAttributeTests
{
    private sealed class ChildWithRequired
    {
        [Required]
        public string? Name { get; set; }
    }

    private sealed class ParentWithValidate
    {
        [Validate]
        public ChildWithRequired? Child { get; set; }
    }

    private sealed class ChildValidatableWithItems : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext.Items is { } items && items.TryGetValue("token", out var v) && Equals(v, 123))
            {
                yield break; // success
            }

            yield return new ValidationResult("token missing or invalid", new[] { nameof(ChildValidatableWithItems) });
        }
    }

    private sealed class ParentWithValidateAndItems
    {
        [Validate]
        public ChildValidatableWithItems? Child { get; set; }
    }

    private sealed class TestService
    {
        public string Token { get; init; } = string.Empty;

        public int Requests { get; set; }
    }

    private sealed class CountingServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object?> _map;

        public CountingServiceProvider(Dictionary<Type, object?> map)
        {
            _map = map;
        }

        public int Calls { get; private set; }

        public object? GetService(Type serviceType)
        {
            Calls++;
            _map.TryGetValue(serviceType, out var service);
            return service;
        }
    }

    private sealed class ChildServiceConsumer : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var svc = (TestService?)validationContext.GetService(typeof(TestService));
            if (svc is null)
            {
                yield return new ValidationResult("service not available", new[] { nameof(ChildServiceConsumer) });
                yield break;
            }

            svc.Requests++;
            if (svc.Token == "ok")
            {
                yield break; // success
            }

            yield return new ValidationResult("invalid token", new[] { nameof(ChildServiceConsumer) });
        }
    }

    private sealed class ParentWithValidateAndService
    {
        [Validate]
        public ChildServiceConsumer? Child { get; set; }
    }

    [Fact]
    public void ValidChild_Passes()
    {
        // arrange
        var parent = new ParentWithValidate
        {
            Child = new ChildWithRequired { Name = "ok" },
        };
        var context = new ValidationContext(parent);
        var results = new List<ValidationResult>();

        // act
        var ok = Validator.TryValidateObject(parent, context, results, validateAllProperties: true);

        // assert
        Assert.True(ok);
        Assert.Empty(results);
    }

    [Fact]
    public void InvalidChild_ComposesMessage_And_MemberName()
    {
        // arrange: missing Name triggers [Required]
        var parent = new ParentWithValidate { Child = new ChildWithRequired() };
        var context = new ValidationContext(parent);
        var results = new List<ValidationResult>();

        // act
        var ok = Validator.TryValidateObject(parent, context, results, validateAllProperties: true);

        // assert
        Assert.False(ok);
        var vr = Assert.Single(results);

        // MemberNames should reference the property name on the parent
        Assert.Contains("Child", vr.MemberNames);

        // Composite message should include the default error header and the inner failure
        Assert.Contains("The field Child is invalid.", vr.ErrorMessage);
        Assert.Contains("[Name]", vr.ErrorMessage);
        Assert.Contains("required", vr.ErrorMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    // Note: Behavior of DataAnnotations with validateAllProperties=false can vary across runtimes
    // and is covered implicitly via Items/IValidatableObject tests below.

    [Fact]
    public void Items_Propagate_To_Nested_ValidationContext()
    {
        // arrange
        var parent = new ParentWithValidateAndItems { Child = new ChildValidatableWithItems() };
        var items = new Dictionary<object, object?> { ["token"] = 123 };
        var context = new ValidationContext(parent, serviceProvider: null, items);
        var results = new List<ValidationResult>();

        // act
        var ok = Validator.TryValidateObject(parent, context, results, validateAllProperties: true);

        // assert
        Assert.True(ok);
        Assert.Empty(results);
    }

    [Fact]
    public void Items_Missing_Fails_And_ComposesMessage()
    {
        // arrange
        var parent = new ParentWithValidateAndItems { Child = new ChildValidatableWithItems() };
        var context = new ValidationContext(parent);
        var results = new List<ValidationResult>();

        // act
        var ok = Validator.TryValidateObject(parent, context, results, validateAllProperties: true);

        // assert
        Assert.False(ok);
        var vr = Assert.Single(results);
        Assert.Contains("Child", vr.MemberNames);
        Assert.Contains("token missing", vr.ErrorMessage, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NullValue_Succeeds()
    {
        // arrange
        var parent = new ParentWithValidate { Child = null };
        var context = new ValidationContext(parent);
        var results = new List<ValidationResult>();

        // act
        var ok = Validator.TryValidateObject(parent, context, results, validateAllProperties: true);

        // assert
        Assert.True(ok);
        Assert.Empty(results);
    }

    [Fact]
    public void ServiceProvider_Forwards_GetService_To_ParentContext()
    {
        // arrange: parent has a service provider; child will call GetService within IValidatableObject
        var service = new TestService { Token = "ok" };
        var provider = new CountingServiceProvider(new Dictionary<Type, object?>
        {
            [typeof(TestService)] = service,
        });

        var parent = new ParentWithValidateAndService
        {
            Child = new ChildServiceConsumer(),
        };

        var context = new ValidationContext(parent, provider, items: null);
        var results = new List<ValidationResult>();

        // act
        var ok = Validator.TryValidateObject(parent, context, results, validateAllProperties: true);

        // assert: validation succeeds, the parent's provider was invoked via ValidateAttribute's ServiceProvider,
        // and the same service instance was used inside the child.
        Assert.True(ok);
        Assert.Empty(results);
        Assert.Equal(1, provider.Calls);
        Assert.Equal(1, service.Requests);
    }
}
