// <copyright file="ImmutableDictionaryYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ImmutableDictionaryYamlTypeConverter : ImmutableCollectionYamlTypeConverter
{
    protected override Type GenericTypeDefinition => typeof(ImmutableDictionary<,>);

    protected override Type ImmutableStaticType => typeof(ImmutableDictionary);

    protected override bool IsDictionary => true;
}
