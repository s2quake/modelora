// <copyright file="ImmutableHashSetYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ImmutableHashSetYamlTypeConverter : ImmutableCollectionYamlTypeConverter
{
    protected override Type GenericTypeDefinition => typeof(ImmutableHashSet<>);

    protected override Type ImmutableStaticType => typeof(ImmutableHashSet);

    protected override bool IsDictionary => false;
}
