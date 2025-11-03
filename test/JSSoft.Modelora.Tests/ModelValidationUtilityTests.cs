// <copyright file="ModelValidationUtilityTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace JSSoft.Modelora.Tests;

public sealed class ModelValidationUtilityTests
{
    private sealed class SimpleRecord
    {
        [Required]
        public string? Name { get; init; }
    }

    private sealed class ItemsConsumerRecord : IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (validationContext.Items is { } items && items.TryGetValue("token", out var v) && Equals(v, 123))
            {
                yield break;
            }

            yield return new ValidationResult("token missing or invalid", [nameof(ItemsConsumerRecord)]);
        }
    }

    private readonly struct RangeStruct
    {
        [Range(1, int.MaxValue)]
        public int Value { get; init; }

        public int Marker { get; init; }
    }

    [Fact]
    public void Validate_Succeeds_When_Valid()
    {
        var obj = new SimpleRecord { Name = "ok" };
        ModelValidationUtility.Validate(obj);
    }

    [Fact]
    public void Validate_Throws_When_Invalid()
    {
        var obj = new SimpleRecord { Name = null };
        Assert.Throws<ValidationException>(() => ModelValidationUtility.Validate(obj));
    }

    [Fact]
    public void Validate_WithItems_Propagates_To_IValidatableObject_Success()
    {
        var obj = new ItemsConsumerRecord();
        var items = new Dictionary<object, object?> { ["token"] = 123 };
        ModelValidationUtility.Validate(obj, items);
    }

    [Fact]
    public void Validate_WithItems_Missing_Fails()
    {
        var obj = new ItemsConsumerRecord();
        var items = new Dictionary<object, object?>();
        Assert.Throws<ValidationException>(() => ModelValidationUtility.Validate(obj, items));
    }

    [Fact]
    public void ValidateAndReturn_Returns_Same_Instance()
    {
        var obj = new SimpleRecord { Name = "ok" };
        var ret = ModelValidationUtility.ValidateAndReturn(obj);
        Assert.Same(obj, ret);
    }

    [Fact]
    public void ValidateAndReturn_WithItems_Returns_Same_Instance()
    {
        var obj = new ItemsConsumerRecord();
        var items = new Dictionary<object, object?> { ["token"] = 123 };
        var ret = ModelValidationUtility.ValidateAndReturn(obj, items);
        Assert.Same(obj, ret);
    }

    [Fact]
    public void TryValidate_Returns_True_And_False()
    {
        var ok = new SimpleRecord { Name = "ok" };
        var bad = new SimpleRecord { Name = null };

        Assert.True(ModelValidationUtility.TryValidate(ok));
        Assert.False(ModelValidationUtility.TryValidate(bad));
    }

    [Fact]
    public void TryValidate_WithResults_Populates_Collection()
    {
        var bad = new SimpleRecord { Name = null };
        var results = new List<ValidationResult>();
        var isValid = ModelValidationUtility.TryValidate(bad, results);
        Assert.False(isValid);
        Assert.NotEmpty(results);
    }

    [Fact]
    public void Validate_ValueType_Default_Skips_Validation()
    {
        // default(RangeStruct) has Value=0 (which violates [Range(1..)]), but default value types are skipped
        var obj = default(RangeStruct);
        ModelValidationUtility.Validate(obj);
    }

    [Fact]
    public void Validate_ValueType_NonDefault_Invalid_Throws()
    {
        // Non-default because Marker=1; Value=0 violates the [Range] attribute and must throw
        var obj = new RangeStruct { Marker = 1, Value = 0 };
        Assert.Throws<ValidationException>(() => ModelValidationUtility.Validate(obj));
    }

    [Fact]
    public void Validate_ValueType_NonDefault_Valid_Succeeds()
    {
        var obj = new RangeStruct { Marker = 1, Value = 1 };
        ModelValidationUtility.Validate(obj);
    }
}
