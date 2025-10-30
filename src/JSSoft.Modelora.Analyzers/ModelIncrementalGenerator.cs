// <copyright file="ModelIncrementalGenerator.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using JSSoft.Modelora.Analyzers.Items;
using Microsoft.CodeAnalysis;

namespace JSSoft.Modelora.Analyzers;

[Generator]
public sealed class ModelIncrementalGenerator : IIncrementalGenerator
{
    private readonly ItemBase[] _items =
    {
        new LIBP1001(),
        new LIBP1002(),
        new LIBP1003(),
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        foreach (var item in _items)
        {
            item.Initialize(context);
        }
    }
}
