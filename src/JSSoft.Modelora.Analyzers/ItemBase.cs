// <copyright file="ItemBase.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using Microsoft.CodeAnalysis;

namespace JSSoft.Modelora.Analyzers;

internal abstract class ItemBase
{
    public abstract void Initialize(IncrementalGeneratorInitializationContext context);
}
