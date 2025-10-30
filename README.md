# Modelora

[![NuGet](https://img.shields.io/nuget/v/JSSoft.Modelora.svg?label=release)](https://www.nuget.org/packages/JSSoft.Modelora/)
[![NuGet (prerelease)](https://img.shields.io/nuget/vpre/JSSoft.Modelora.svg?label=preview)](https://www.nuget.org/packages/JSSoft.Modelora/)
[![License](https://img.shields.io/github/license/s2quake/randora.svg)](https://github.com/s2quake/randora/blob/main/LICENSE.md)

A high-performance model serialization and conversion library for .NET. Efficiently serialize and deserialize complex object models to binary, JSON, and YAML formats.

## Supported Frameworks

This library is packaged as a multi-targeted NuGet package.

- .NET 9.0 (default target for development)
- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET Standard 2.1

## Key Features

- **High-performance binary serialization**: Compact and fast binary format
- **JSON support**: JSON serialization based on System.Text.Json
- **YAML support**: YAML serialization based on YamlDotNet
- **Type safety**: Type safety guarantee through compile-time code generation
- **Version compatibility**: Model version management and compatibility support
- **Custom converters**: Support for user-defined type converters

## Quick Start

### Basic Usage

```csharp
using JSSoft.Modelora;

// Define model class
[Model("Person", Version = 1)]
public partial class Person
{
    [Property(0)]
    public string Name { get; set; } = string.Empty;
    
    [Property(1)]
    public int Age { get; set; }
    
    [Property(2)]
    public DateTime BirthDate { get; set; }
}

// Binary serialization
var person = new Person { Name = "John Doe", Age = 30, BirthDate = new DateTime(1994, 1, 1) };
byte[] data = ModelSerializer.Serialize(person);

// Binary deserialization
var deserializedPerson = ModelSerializer.Deserialize<Person>(data);
```

### JSON Serialization

```csharp
using JSSoft.Modelora.Json;

// JSON serialization
string json = ModelJsonSerializer.Serialize(person);

// JSON deserialization
var personFromJson = ModelJsonSerializer.Deserialize<Person>(json);
```

### YAML Serialization

```csharp
using JSSoft.Modelora.Yaml;

// YAML serialization
string yaml = ModelYamlSerializer.Serialize(person);

// YAML deserialization
var personFromYaml = ModelYamlSerializer.Deserialize<Person>(yaml);
```

## Advanced Features

### Model Attributes

```csharp
// Model with version management
[ModelHistory(Version = 1, Type = typeof(Version1_Person))]
[ModelHistory(Version = 2, Type = typeof(Version2_Person))]
[Model("Person", Version = 3)]
public sealed record class Person
{
    public Person()
    {
    }

    public Person(Version2_Person legacyModel)
    {
        Name = legacyModel.Name;
        Age = legacyModel.Age;
        Email = "default@example.com"; // Default value for newly added field
    }

    [Property(0)]
    public string Name { get; set; } = string.Empty;
    
    [Property(1)]
    public int Age { get; set; }
    
    [Property(2)]
    public string Email { get; set; } = string.Empty;
}

// Legacy version models
[OriginModel(Type = typeof(Person))]
public sealed record class Version1_Person
{
    [Property(0)]
    public string Name { get; set; } = string.Empty;
}

[OriginModel(Type = typeof(Person))]
public sealed record class Version2_Person
{
    public Version2_Person()
    {
    }

    public Version2_Person(Version1_Person legacyModel)
    {
        Name = legacyModel.Name;
        Age = 0; // Default value
    }

    [Property(0)]
    public string Name { get; set; } = string.Empty;
    
    [Property(1)]
    public int Age { get; set; }
}
```

### Scalar Value Definition

```csharp
// Define scalar value type
[ModelScalar("hex_value", Kind = ModelScalarKind.Hex)]
public readonly partial record struct HexValue(in ImmutableArray<byte> Bytes)
    : IEquatable<HexValue>, IComparable<HexValue>, IComparable
{
    public HexValue(ReadOnlySpan<byte> bytes)
        : this(bytes.ToImmutableArray())
    {
    }

    public ImmutableArray<byte> Bytes => _bytes.IsDefault ? ImmutableArray.Create(Array.Empty<byte>()) : _bytes;

    public static HexValue Parse(string hex)
    {
        ImmutableArray<byte> bytes = [.. ByteUtility.Parse(hex)];
        return new HexValue(bytes);
    }

    public override string ToString() => ByteUtility.Hex(Bytes.AsSpan());

    // Scalar conversion methods
    internal byte[] ToScalarValue() => [.. Bytes];

    internal static HexValue FromScalarValue(IServiceProvider serviceProvider, byte[] value)
        => new(value.ToImmutableArray());
}
```

### Custom Converters

```csharp
[ModelConverter(typeof(CustomConverter))]
public class CustomType
{
    public string Value { get; set; } = string.Empty;
}

public class CustomConverter : IModelConverter<CustomType>
{
    public CustomType Read(BinaryReader reader, ModelOptions options)
    {
        return new CustomType { Value = reader.ReadString() };
    }

    public void Write(BinaryWriter writer, CustomType value, ModelOptions options)
    {
        writer.Write(value.Value);
    }
}
```

## Package Structure

- **JSSoft.Modelora**: Core binary serialization library
- **JSSoft.Modelora.Json**: JSON serialization extension
- **JSSoft.Modelora.Yaml**: YAML serialization extension
- **JSSoft.Modelora.Analyzers**: Compile-time code generator

## Installation

```bash
# Core library
dotnet add package JSSoft.Modelora

# JSON support
dotnet add package JSSoft.Modelora.Json

# YAML support
dotnet add package JSSoft.Modelora.Yaml
```
