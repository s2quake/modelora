// <copyright file="ImmutableSortedDictionaryYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ImmutableSortedDictionaryYamlTypeConverter : ImmutableCollectionYamlTypeConverter
{
    protected override Type GenericTypeDefinition => typeof(ImmutableSortedDictionary<,>);

    protected override Type ImmutableStaticType => typeof(ImmutableSortedDictionary);

    protected override bool IsDictionary => true;
}
