// <copyright file="ModelYamlSerializerTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using JSSoft.Modelora.Tests;

namespace JSSoft.Modelora.Yaml.Tests;

public sealed partial class ModelYamlSerializerTest(ITestOutputHelper output)
    : ModelSerializerTestBase<string>(output)
{
    protected override object? Deserialize(string serialized, Type type, ModelOptions options)
        => ModelYamlSerializer.Deserialize(serialized, type, options);

    protected override string Serialize(object? obj, Type type, ModelOptions options)
        => ModelYamlSerializer.Serialize(obj, type, options);
}
