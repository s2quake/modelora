# Benchmarks: Modelora (binary) vs MessagePack

This benchmark compares serialization speed and payload size between:
- Modelora binary (`JSSoft.Modelora.ModelSerializer`)
- MessagePack (`MessagePack`)

It defines a simple data model (`SampleData`) annotated for both frameworks and runs microbenchmarks with BenchmarkDotNet. Runs are shortened using a ShortRun job.

## How to run

```bash
# From repo root
cd benchmark/Modelora.Benchmarks

# Restore and run benchmarks in Release
dotnet run -c Release -- --filter "*SerializationBenchmarks*"
```

On startup, it prints a quick size preview (byte lengths) for small/medium/large instances, then runs short microbenchmarks.

## Notes
- Both serializers produce byte arrays; we report their sizes using `.Length`.
- The model type is shared and carries both Modelora and MessagePack attributes to avoid double definitions.
