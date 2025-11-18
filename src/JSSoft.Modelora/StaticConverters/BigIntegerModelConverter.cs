// <copyright file="BigIntegerModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class BigIntegerModelConverter : ModelConverter<BigInteger>
{
    protected override BigInteger Read(ref ModelReader reader, Type type, ModelOptions options)
        => reader.ReadBigInteger();

    protected override void Write(ref ModelWriter writer, BigInteger value, ModelOptions options)
        => writer.Write(value);
}
