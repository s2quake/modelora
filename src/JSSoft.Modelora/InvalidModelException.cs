// <copyright file="InvalidModelException.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

public class InvalidModelException : ModelException
{
    public InvalidModelException(string message, Type modelType)
        : base(message)
    {
        ModelType = modelType;
    }

    public InvalidModelException(string message, Type modelType, Exception innerException)
        : base(message, innerException)
    {
        ModelType = modelType;
    }

    public Type ModelType { get; }
}
