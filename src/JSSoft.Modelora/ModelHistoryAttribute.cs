// <copyright file="ModelHistoryAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using System.Reflection;
using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ModelHistoryAttribute : Attribute
{
    [NonNegative]
    public required int Version { get; init; }

    public required Type Type { get; init; }

    internal void Validate(Type modelType, int previousVersion, Type? previousType)
    {
        var validationContext = new ValidationContext(this);
        Validator.ValidateObject(this, validationContext, validateAllProperties: true);
        var version = Version;
        if (version != previousVersion + 1)
        {
            throw new InvalidModelException($"The version of type '{Type}' must be {previousVersion + 1}.", modelType);
        }

        if (Type.GetCustomAttribute<OriginModelAttribute>() is not { } originModelAttribute)
        {
            throw new InvalidModelException($"Type '{Type}' does not have the {nameof(OriginModelAttribute)}.", modelType);
        }

        if (originModelAttribute.Type != modelType)
        {
            var message = $"{nameof(OriginModelAttribute)}.{nameof(OriginModelAttribute.Type)} of " +
                          $"'{Type}' must be '{modelType}'.";
            throw new InvalidModelException(message, modelType);
        }

        if (previousType is not null)
        {
            if (Type.GetConstructor([previousType]) is null)
            {
                var message = $"Type '{Type}' does not have a constructor with a single parameter of type " +
                              $"'{previousType}'.";
                throw new InvalidModelException(message, modelType);
            }

            if (Type.GetConstructor([]) is null)
            {
                throw new InvalidModelException($"Type '{Type}' does not have a default constructor.", modelType);
            }
        }
    }
}
