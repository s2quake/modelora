// <copyright file="PropertyAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyAttribute(int index) : Attribute
{
    public int Index => index;

    public bool EmitDefaultValue { get; init; }

    public bool InspectOnly { get; init; }
}
