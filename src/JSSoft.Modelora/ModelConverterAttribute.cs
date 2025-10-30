// <copyright file="ModelConverterAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class ModelConverterAttribute(Type converterType, string typeName) : Attribute
{
    public Type ConverterType { get; } = converterType;

    public string TypeName { get; } = typeName;
}
