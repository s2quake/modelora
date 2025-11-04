// <copyright file="ModelSerializerTestBase.Abstract.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
{
    [Fact]
    public void RecordClassWithAbstractProperty_SerializeAndDeserialize_Test()
    {
        var expectedObject = new RecordClassWithAbstractProperty
        {
            Derived = new DerivedRecordClass
            {
                ParentValue1 = 100,
                Value1 = 200,
            },
            DerivedList =
            [
                new DerivedRecordClass
                {
                    ParentValue1 = 300,
                    Value1 = 400,
                },
                new DerivedRecordClass
                {
                    ParentValue1 = 500,
                    Value1 = 600,
                },
            ],
        };
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithAbstractProperty>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void RecordClassWithAbstractPropertyNotModel1_SerializeAndDeserialize_Test()
    {
        var expectedObject = new RecordClassWithAbstractPropertyNotModel1();
        Assert.Throws<InvalidModelException>(() => Serialize(expectedObject));
    }

    [Fact]
    public void RecordClassWithAbstractPropertyNotModel2_SerializeAndDeserialize_Test()
    {
        var expectedObject = new RecordClassWithAbstractPropertyNotModel2();
        Assert.Throws<InvalidModelException>(() => Serialize(expectedObject));
    }
}

[Model("JSSoft_Modelora_Tests_ModelSerializerTest_RecordClassWithAbstractProperty", Version = 1)]
public sealed partial record class RecordClassWithAbstractProperty : IEquatable<RecordClassWithAbstractProperty>
{
    [Property(0)]
    public required AbstractRecordClass Derived { get; init; }

    [Property(1)]
    [NotEmpty]
    public ImmutableArray<AbstractRecordClass> DerivedList { get; init; } = [];
}

[Model("JSSoft_Modelora_Tests_ModelSerializerTest_AbstractRecordClass", Version = 1)]
public abstract record class AbstractRecordClass : IEquatable<AbstractRecordClass>
{
    [Property(0)]
    public int ParentValue1 { get; init; }
}

[Model("JSSoft_Modelora_Tests_ModelSerializerTest_DerivedRecordClass", Version = 1)]
public sealed record class DerivedRecordClass : AbstractRecordClass, IEquatable<DerivedRecordClass>
{
    [Property(0)]
    public int Value1 { get; init; }
}

public abstract record class AbstractRecordClassNotModel : IEquatable<AbstractRecordClassNotModel>
{
    [Property(0)]
    public int ParentValue1 { get; init; }
}

[Model("JSSoft_Modelora_Tests_ModelSerializerTest_RecordClassWithAbstractPropertyNotModel1", Version = 1)]
public sealed partial record class RecordClassWithAbstractPropertyNotModel1
    : IEquatable<RecordClassWithAbstractPropertyNotModel1>
{
    [Property(0)]
    public AbstractRecordClassNotModel? Derived { get; init; }
}

[Model("JSSoft_Modelora_Tests_ModelSerializerTest_RecordClassWithAbstractPropertyNotModel2", Version = 1)]
public sealed partial record class RecordClassWithAbstractPropertyNotModel2
    : IEquatable<RecordClassWithAbstractPropertyNotModel2>
{
    [Property(0)]
    [NotEmpty]
    public ImmutableArray<AbstractRecordClassNotModel> DerivedList { get; init; } = [];
}
