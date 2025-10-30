// <copyright file="MethodDeclarationSyntaxExtensions.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JSSoft.Modelora.Analyzers;

internal static class MethodDeclarationSyntaxExtensions
{
    public static bool IsStatic(this MethodDeclarationSyntax @this)
        => @this.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));
}
