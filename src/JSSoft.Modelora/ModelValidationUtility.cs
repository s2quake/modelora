// <copyright file="ModelValidationUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace JSSoft.Modelora;

public static class ModelValidationUtility
{
    public static void Validate(object obj)
    {
        var type = obj.GetType();
        if (!type.IsValueType || !TypeUtility.IsDefault(obj))
        {
            Validator.ValidateObject(instance: obj, new(obj), true);
        }
    }

    public static void Validate(object obj, IDictionary<object, object?> items)
    {
        var type = obj.GetType();
        if (!type.IsValueType || !TypeUtility.IsDefault(obj))
        {
            Validator.ValidateObject(instance: obj, new(obj, items), true);
        }
    }

    public static T ValidateAndReturn<T>(T obj)
        where T : notnull
    {
        Validate(obj);
        return obj;
    }

    public static T ValidateAndReturn<T>(T obj, IDictionary<object, object?> items)
        where T : notnull
    {
        Validate(obj, items);
        return obj;
    }

    public static bool TryValidate(object obj)
        => Validator.TryValidateObject(obj, new(obj), null, true);

    public static bool TryValidate(object obj, ICollection<ValidationResult>? validationResults)
        => Validator.TryValidateObject(obj, new(obj), validationResults, true);
}
