// <copyright file="ModelCreationException.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

public sealed class ModelCreationException : ModelException
{
    public ModelCreationException(Type type)
        : base(GetMessage(type))
    {
    }

    public ModelCreationException(Type type, Exception innerException)
        : base(GetMessage(type), innerException)
    {
    }

    private static string GetMessage(Type type)
        => $"Failed to create model for type '{type.FullName}'.";
}
