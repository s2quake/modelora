// <copyright file="ImmutableSortedSetModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class ImmutableSortedSetModelConverter : ImmutableCollectionModelConverter
{
    protected override Type ImmutableStaticType => typeof(ImmutableSortedSet);

    protected override Type GenericTypeDefinition => typeof(ImmutableSortedSet<>);

    protected override bool IsDictionary => false;
}
