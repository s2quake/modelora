// <copyright file="NotDefaultAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace JSSoft.Modelora.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NotDefaultAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var memberName = validationContext.DisplayName;
        var declaringType = validationContext.ObjectType;

        if (value is null)
        {
            var message = $"The value specified in {GetType().Name} on " +
                          $"{declaringType.Name}.{memberName} must not be null.";
            return new ValidationResult(message, [memberName]);
        }

        var valueType = value.GetType();
        if (!valueType.IsValueType)
        {
            var message = $"The type {valueType.Name} specified in {GetType().Name} on " +
                          $"{declaringType.Name}.{memberName} must be a value type.";
            return new ValidationResult(message, [memberName]);
        }
        else if (TypeUtility.IsDefault(value))
        {
            var message = $"The value specified in {GetType().Name} on " +
                          $"{declaringType.Name}.{memberName} must not be the default value.";
            return new ValidationResult(message, [memberName]);
        }

        return ValidationResult.Success;
    }
}
