// <copyright file="ImmutableArrayYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ImmutableArrayYamlTypeConverter : ImmutableCollectionYamlTypeConverter
{
    protected override Type GenericTypeDefinition => typeof(ImmutableArray<>);

    protected override Type ImmutableStaticType => typeof(ImmutableArray);

    protected override bool IsDictionary => false;
}
