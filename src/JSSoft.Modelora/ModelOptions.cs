// <copyright file="ModelOptions.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora;

public sealed record class ModelOptions : IServiceProvider
{
    private readonly IServiceProvider? _serviceProvider;

    public ModelOptions()
    {
    }

    public ModelOptions(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public static ModelOptions Empty { get; } = new();

    public bool IsValidationEnabled { get; init; }

    public SerializationPurpose Purpose { get; init; }

    public TypeInfoEmission TypeInfoEmission { get; init; }

    public bool EmitDefaultValues { get; init; }

    public object? GetService(Type serviceType) => _serviceProvider?.GetService(serviceType);
}
