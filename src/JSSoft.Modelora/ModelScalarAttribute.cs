// <copyright file="ModelScalarAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using JSSoft.Modelora.DynamicConverters;

namespace JSSoft.Modelora;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class ModelScalarAttribute(string typeName)
    : ModelConverterAttribute(typeof(ModelScalarConverter), typeName)
{
    public ModelScalarKind Kind { get; init; }

    public string DisplayName { get; init; } = string.Empty;
}
