// <copyright file="SortedDictionaryModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class SortedDictionaryModelConverter : CollectionModelConverter
{
    protected override Type GenericTypeDefinition => typeof(SortedDictionary<,>);

    protected override bool IsDictionary => true;

    protected override IEnumerable CreateInstance(Type type, Type elementType, IList listInstance)
    {
        var dictionaryType = typeof(Dictionary<,>).MakeGenericType(elementType.GetGenericArguments());
        var dictionary = (ICollection)Activator.CreateInstance(dictionaryType, [listInstance])!;
        return (IEnumerable)Activator.CreateInstance(type, [dictionary])!;
    }
}
