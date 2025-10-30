// <copyright file="ArrayModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.Diagnostics;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class ArrayModelConverter : CollectionModelConverter
{
    protected override Type GenericTypeDefinition
        => throw new UnreachableException("Array does not have a generic type definition.");

    protected override bool IsDictionary => false;

    public override bool CanConvert(Type type) => typeof(Array).IsAssignableFrom(type);

    protected override IEnumerable CreateInstance(Type type, Type elementType, IList listInstance)
    {
        var array = Array.CreateInstance(type.GetElementType()!, listInstance.Count);
        listInstance.CopyTo(array, 0);
        return array;
    }

    protected override Type GetElementType(Type type) => type.GetElementType()
        ?? throw new ArgumentException($"Cannot get the element type from {type}.", nameof(type));
}
