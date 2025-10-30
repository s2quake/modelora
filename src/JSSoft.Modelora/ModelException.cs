// <copyright file="ModelException.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

public class ModelException : Exception
{
    public ModelException(string message)
        : base(message)
    {
    }

    public ModelException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
