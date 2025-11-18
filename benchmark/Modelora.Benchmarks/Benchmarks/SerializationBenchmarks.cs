// <copyright file="SerializationBenchmarks.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using BenchmarkDotNet.Attributes;
using JSSoft.Modelora;
using MessagePack;
using Modelora.Benchmarks.Models;

namespace Modelora.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[HideColumns("Error", "StdDev", "Median")]
public class SerializationBenchmarks
{
    private readonly ModelOptions _options = new ModelOptions
    {
        TypeInfoEmission = TypeInfoEmission.Never,
    };

    private SampleData _data = default!;
    private byte[] _modelBytes = Array.Empty<byte>();
    private byte[] _mpBytes = Array.Empty<byte>();

    [Params(16, 128)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _data = SampleData.Create(seed: 42, count: Count);
        _modelBytes = ModelSerializer.Serialize(_data, _options);
        _mpBytes = MessagePackSerializer.Serialize(_data);
    }

    [Benchmark]
    public byte[] Serialize_Modelora() => ModelSerializer.Serialize(_data, _options);

    [Benchmark]
    public byte[] Serialize_MessagePack() => MessagePackSerializer.Serialize(_data);

    [Benchmark]
    public SampleData Deserialize_Modelora() => ModelSerializer.Deserialize<SampleData>(_modelBytes, _options);

    [Benchmark]
    public SampleData Deserialize_MessagePack() => MessagePackSerializer.Deserialize<SampleData>(_mpBytes);

    [Benchmark]
    public int Size_Modelora() => _modelBytes.Length;

    [Benchmark]
    public int Size_MessagePack() => _mpBytes.Length;
}
