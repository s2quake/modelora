// <copyright file="ImmutableArrayJsonConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Json.DynamicConverters;

internal sealed class ImmutableArrayJsonConverter : ImmutableCollectionJsonConverter
{
    protected override Type GenericTypeDefinition => typeof(ImmutableArray<>);

    protected override Type ImmutableStaticType => typeof(ImmutableArray);

    protected override bool IsDictionary => false;
}
