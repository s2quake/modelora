// <copyright file="IModelComparer.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

internal interface IModelComparer
{
    bool Equals(object obj1, object obj2, Type type);

    int GetHashCode(object obj, Type type);
}
