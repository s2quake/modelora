// <copyright file="ArrayYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.Diagnostics;

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ArrayYamlTypeConverter : CollectionYamlTypeConverter
{
    protected override Type GenericTypeDefinition
        => throw new UnreachableException("Array does not have a generic type definition.");

    protected override bool IsDictionary => false;

    public override bool Accepts(Type type) => typeof(Array).IsAssignableFrom(type);

    protected override IEnumerable CreateInstance(Type typeToConvert, Type elementType, IList listInstance)
    {
        var array = Array.CreateInstance(typeToConvert.GetElementType()!, listInstance.Count);
        listInstance.CopyTo(array, 0);
        return array;
    }

    protected override Type GetElementType(Type typeToConvert) => typeToConvert.GetElementType()
        ?? throw new ArgumentException($"Cannot get the element type from {typeToConvert}.", nameof(typeToConvert));
}
