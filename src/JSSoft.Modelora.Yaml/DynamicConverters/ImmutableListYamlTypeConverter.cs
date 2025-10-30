// <copyright file="ImmutableListYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ImmutableListYamlTypeConverter : ImmutableCollectionYamlTypeConverter
{
    protected override Type GenericTypeDefinition => typeof(ImmutableList<>);

    protected override Type ImmutableStaticType => typeof(ImmutableList);

    protected override bool IsDictionary => false;
}
