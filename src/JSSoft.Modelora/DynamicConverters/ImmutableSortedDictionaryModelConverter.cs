// <copyright file="ImmutableSortedDictionaryModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class ImmutableSortedDictionaryModelConverter : ImmutableCollectionModelConverter
{
    protected override Type ImmutableStaticType => typeof(ImmutableSortedDictionary);

    protected override Type GenericTypeDefinition => typeof(ImmutableSortedDictionary<,>);

    protected override bool IsDictionary => true;
}
