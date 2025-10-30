// <copyright file="ModelKnownTypeAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ModelKnownTypeAttribute(Type type, string typeName) : Attribute
{
    public Type Type { get; } = type;

    public string TypeName { get; } = typeName;

    public string DisplayName { get; init; } = string.Empty;
}
