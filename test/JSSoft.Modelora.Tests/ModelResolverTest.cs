// <copyright file="ModelResolverTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace JSSoft.Modelora.Tests;

public sealed class ModelResolverTest
{
    [Fact]
    public void GetHashCode_Test()
    {
        var model1 = new Model
        {
            Version = 1,
            Values1 = [0, 1, 2],
            Values2 =
            [
                [0, 1, 2],
                [3, 4, 5],
                [6, 7, 8],
            ],
            ImmutableValues1 = [0, 1, 2],
            ImmutableValues2 =
            [
                [6, 7, 8],
                [3, 4, 5],
                [0, 1, 2],
            ],
        };
        var model2 = new Model
        {
            Version = 1,
            Values1 = [0, 1, 2],
            Values2 =
            [
                [0, 1, 2],
                [3, 4, 5],
                [6, 7, 8],
            ],
            ImmutableValues1 = [0, 1, 2],
            ImmutableValues2 =
            [
                [6, 7, 8],
                [3, 4, 5],
                [0, 1, 2],
            ],
        };

        var hash1 = ModelResolver.GetHashCode(model1);
        var hash2 = ModelResolver.GetHashCode(model2);

        Assert.Equal(hash1, hash2);
        Assert.True(model1.Equals(model2));

        Assert.Equal(0, ModelResolver.GetHashCode(null, typeof(Model)));
    }

    [Fact]
    public void Equals_Test()
    {
        var model1 = new Model
        {
            Version = 1,
            Values1 = [0, 1, 2],
            Values2 =
            [
                [0, 1, 2],
                [3, 4, 5],
                [6, 7, 8],
            ],
            ImmutableValues1 = [0, 1, 2],
            ImmutableValues2 =
            [
                [6, 7, 8],
                [3, 4, 5],
                [0, 1, 2],
            ],
        };
        var model2 = new Model
        {
            Version = 1,
            Values1 = [0, 1, 2],
            Values2 =
            [
                [0, 1, 2],
                [3, 4, 5],
                [6, 7, 8],
            ],
            ImmutableValues1 = [0, 1, 2],
            ImmutableValues2 =
            [
                [6, 7, 8],
                [3, 4, 5],
                [0, 1, 2],
            ],
        };

        Assert.True(ModelResolver.Equals(model1, model2));
        Assert.False(ModelResolver.Equals(null, model2, typeof(Model)));
        Assert.False(ModelResolver.Equals(model1, null, typeof(Model)));
        Assert.False(ModelResolver.Equals(0, model2, typeof(Model)));
        Assert.False(ModelResolver.Equals(model1, 0, typeof(Model)));
    }

    [Fact]
    public void GetType_WithVersionHistory_Works()
    {
        Assert.Equal(typeof(ModelRecord), ModelResolver.GetType(typeof(ModelRecord), 0));
        Assert.Equal(typeof(Version1_ModelRecord), ModelResolver.GetType(typeof(ModelRecord), 1));
        Assert.Equal(typeof(Version2_ModelRecord), ModelResolver.GetType(typeof(ModelRecord), 2));
        Assert.Equal(typeof(ModelRecord), ModelResolver.GetType(typeof(ModelRecord), 3));
    }

    [Fact]
    public void GetType_OutOfRange_Throws_ModelException()
    {
        Assert.Throws<ModelException>(() => ModelResolver.GetType(typeof(ModelRecord), 4));
        Assert.Throws<ModelException>(() => ModelResolver.GetType(typeof(ModelRecord), -1));
    }

    [Fact]
    public void GetTypeInfo_ForModel_And_OriginModel()
    {
        var (name1, version1) = ModelResolver.GetTypeInfo(typeof(ModelRecord));
        Assert.Equal(TypeUtility.GetTypeName(typeof(ModelRecord)), name1);
        Assert.Equal(3, version1);

        var (name2, version2) = ModelResolver.GetTypeInfo(typeof(Version1_ModelRecord));
        Assert.Equal(TypeUtility.GetTypeName(typeof(ModelRecord)), name2);
        Assert.Equal(1, version2);

        var (name3, version3) = ModelResolver.GetTypeInfo(typeof(Version2_ModelRecord));
        Assert.Equal(TypeUtility.GetTypeName(typeof(ModelRecord)), name3);
        Assert.Equal(2, version3);
    }

    [Fact]
    public void GetTypeInfo_ForKnownType_HasVersionZero()
    {
        var (name, version) = ModelResolver.GetTypeInfo(typeof(int));
        Assert.Equal(TypeUtility.GetTypeName(typeof(int)), name);
        Assert.Equal(0, version);
    }

    [Fact]
    public void GetTypeInfo_ForUnsupportedType_Throws_InvalidModelException()
    {
        var message = $"Type '{typeof(NotHasModelConverter)}' is not supported or not registered in known types.";
        var e = Assert.Throws<InvalidModelException>(() => ModelResolver.GetTypeInfo(typeof(NotHasModelConverter)));
        Assert.Equal(message, e.Message);
        Assert.Equal(typeof(NotHasModelConverter), e.ModelType);
    }

    [Fact]
    public void GetVersion_Works_And_Throws_For_Unsupported()
    {
        Assert.Equal(3, ModelResolver.GetVersion(typeof(ModelRecord)));
        Assert.Equal(2, ModelResolver.GetVersion(typeof(Version2_ModelRecord)));
        Assert.Equal(1, ModelResolver.GetVersion(typeof(Version1_ModelRecord)));
        Assert.Equal(0, ModelResolver.GetVersion(typeof(int)));

        var message = $"Type '{typeof(NotHasModelConverter)}' is not supported or not registered in known types.";
        var e = Assert.Throws<InvalidModelException>(() => ModelResolver.GetVersion(typeof(NotHasModelConverter)));
        Assert.Equal(message, e.Message);
    }

    [Fact]
    public void GetVersion_WithOriginModel_TargetWithoutModelAttribute_Throws()
    {
        var message = $"Type '{typeof(OriginTarget_NoModel)}' does not have the ModelAttribute.";
        var e = Assert.Throws<InvalidModelException>(() => ModelResolver.GetVersion(typeof(Origin_NoModelTarget)));
        Assert.Equal(message, e.Message);
    }

    [Fact]
    public void GetProperties_Order_Is_By_Index()
    {
        var props = ModelResolver.GetProperties(typeof(ModelRecord));
        Assert.Equal(2, props.Count);
        Assert.Equal("String", props[0].Name);
        Assert.Equal("Int", props[1].Name);
    }

    [Fact]
    public void GetProperties_ForNonModel_Throws_ModelException()
    {
        Assert.Throws<ModelException>(() => ModelResolver.GetProperties(typeof(NotHasModelConverter)));
    }

    [Fact]
    public void TryGetConverter_Finds_Converters_For_Common_Types()
    {
        Assert.True(ModelResolver.TryGetConverter(typeof(int), out var conv1));
        Assert.NotNull(conv1);

        Assert.True(ModelResolver.TryGetConverter(typeof(Tuple<int, bool>), out var conv2));
        Assert.NotNull(conv2);

        Assert.True(ModelResolver.TryGetConverter(typeof(KeyValuePair<int, string>), out var conv3));
        Assert.NotNull(conv3);

        Assert.True(ModelResolver.TryGetConverter(typeof(ImmutableArray<int>), out var conv4));
        Assert.NotNull(conv4);
    }

    [Fact]
    public void TryGetConverter_ForUnsupportedType_ReturnsFalse()
    {
        var ok = ModelResolver.TryGetConverter(typeof(NotHasModelConverter), out var conv);
        Assert.False(ok);
        Assert.Null(conv);
    }

    [Fact]
    public void GetConverter_From_ModelConverterAttribute_Works()
    {
        var converter = ModelResolver.GetConverter(typeof(HasModelConverter));
        Assert.IsType<HasModelConverterModelConverter>(converter);
    }

    [Fact]
    public void GetConverter_WithoutAttribute_Throws()
    {
        Assert.Throws<InvalidModelException>(() => ModelResolver.GetConverter(typeof(NotHasModelConverter)));
    }

    [Fact]
    public void Equals_And_HashCode_Work_For_KeyValuePair()
    {
        var kv1 = new KeyValuePair<int, string>(1, "a");
        var kv2 = new KeyValuePair<int, string>(1, "a");
        var kv3 = new KeyValuePair<int, string>(2, "b");

        Assert.True(ModelResolver.Equals(kv1, kv2));
        Assert.False(ModelResolver.Equals(kv1, kv3));

        var h1 = ModelResolver.GetHashCode(kv1);
        var h2 = ModelResolver.GetHashCode(kv2);
        var h3 = ModelResolver.GetHashCode(kv3);

        Assert.Equal(h1, h2);
        Assert.NotEqual(h1, h3);
    }

    [Fact]
    public void Validate_Uses_DataAnnotations()
    {
        var ok = new AnnotatedRecord { Name = "ok" };
        ModelResolver.Validate(ok, new ModelOptions());

        var invalid = new AnnotatedRecord { Name = null };
        Assert.Throws<ValidationException>(() => ModelResolver.Validate(invalid, new ModelOptions()));
    }
}

[Model("JSSoft_Modelora_Tests_ModelResolverTest_Model", Version = 1)]
public sealed record class Model : IEquatable<Model>
{
    [Property(0)]
    public int Version { get; set; }

    [Property(1)]
    public int[] Values1 { get; set; } = [];

    [Property(2)]
    public int[][] Values2 { get; set; } = [];

    [Property(3)]
    public ImmutableArray<int> ImmutableValues1 { get; set; } = [];

    [Property(4)]
    public ImmutableArray<int[]> ImmutableValues2 { get; set; } = [];

    public bool Equals(Model? other) => ModelResolver.Equals(this, other);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

[Model("JSSoft_Modelora_Tests_ModelResolverTests_AnnotatedRecord", Version = 1)]
public sealed record class AnnotatedRecord
{
    [Property(0)]
    [Required]
    public string? Name { get; init; }
}

[OriginModel(Type = typeof(OriginTarget_NoModel))]
public sealed record class Origin_NoModelTarget
{
    public int Value { get; init; }
}

public sealed record class OriginTarget_NoModel
{
    public int Value { get; init; }
}
