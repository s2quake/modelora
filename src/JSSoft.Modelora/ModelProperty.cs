// <copyright file="ModelProperty.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;

namespace JSSoft.Modelora;

public sealed class ModelProperty
{
    private readonly PropertyAttribute _propertyAttribute;
    private readonly PropertyInfo _propertyInfo;

    internal ModelProperty(PropertyAttribute propertyAttribute, PropertyInfo propertyInfo)
    {
        _propertyAttribute = propertyAttribute;
        _propertyInfo = propertyInfo;
    }

    public bool EmitDefaultValue => _propertyAttribute.EmitDefaultValue;

    public bool InspectOnly => _propertyAttribute.InspectOnly;

    public Type PropertyType => _propertyInfo.PropertyType;

    public string Name => _propertyInfo.Name;

    public object? GetValue(object obj) => _propertyInfo.GetValue(obj);

    public void SetValue(object obj, object? value) => _propertyInfo.SetValue(obj, value);
}
