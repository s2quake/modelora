// <copyright file="NotEmptyAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.ComponentModel.DataAnnotations;
using static JSSoft.Modelora.TypeUtility;

namespace JSSoft.Modelora.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NotEmptyAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var memberName = validationContext.DisplayName;
        var declaringType = validationContext.ObjectType;

        if (value is ICollection collection)
        {
            var valueType = value.GetType();
            if (valueType.IsValueType && IsDefault(value))
            {
                var message = $"The value specified in {GetType().Name} on " +
                              $"{declaringType.Name}.{memberName} must not be the default value.";
                return new ValidationResult(message, [memberName]);
            }

            if (collection.Count > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
                $"The collection specified in {GetType().Name} on " +
                $"{declaringType.Name}.{memberName} must not be empty.",
                [memberName]);
        }
        else if (value is IEnumerable enumerable)
        {
            var valueType = value.GetType();
            if (valueType.IsValueType && IsDefault(value))
            {
                var message = $"The value specified in {GetType().Name} on " +
                              $"{declaringType.Name}.{memberName} must not be the default value.";
                return new ValidationResult(message, [memberName]);
            }

            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
                $"The collection specified in {GetType().Name} on " +
                $"{declaringType.Name}.{memberName} must not be empty.",
                [memberName]);
        }
        else
        {
            var message = $"The value specified in {GetType().Name} on " +
                          $"{declaringType.Name}.{memberName} must be convertible to IEnumerable.";
            return new ValidationResult(message, [memberName]);
        }
    }
}
