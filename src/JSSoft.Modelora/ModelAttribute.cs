// <copyright file="ModelAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;
using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
public sealed class ModelAttribute(string typeName) : Attribute
{
    [NotEmpty]
    public string TypeName { get; } = typeName;

    [NonNegative]
    public required int Version { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    internal void Validate(Type modelType, int previousVersion, Type? previousType)
    {
        var validationContext = new ValidationContext(this);
        Validator.ValidateObject(this, validationContext, validateAllProperties: true);

        if (Version != previousVersion + 1)
        {
            throw new InvalidModelException($"The version of type '{modelType}' must be {previousVersion + 1}.", modelType);
        }

        if (previousType is not null)
        {
            if (modelType.GetConstructor([previousType]) is null)
            {
                var message = $"Type '{modelType}' does not have a constructor with a single parameter of type " +
                              $"'{previousType}'.";
                throw new InvalidModelException(message, modelType);
            }

            if (modelType.GetConstructor([]) is null)
            {
                throw new InvalidModelException($"Type '{modelType}' does not have a default constructor.", modelType);
            }
        }
    }
}
