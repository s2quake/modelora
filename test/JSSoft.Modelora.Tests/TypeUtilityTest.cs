// <copyright file="TypeUtilityTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

public sealed class TypeUtilityTest
{
    public sealed class HasArgs(int x, string y)
    {
        public int X { get; } = x;
        public string Y { get; } = y;
    }

    public sealed class CtorThrows
    {
        public CtorThrows()
        {
            throw new InvalidOperationException("boom");
        }
    }

    [Theory]
    [InlineData(typeof(BigInteger), "bi")]
    [InlineData(typeof(BigInteger?), "bi?")]
    [InlineData(typeof(bool), "b")]
    [InlineData(typeof(bool?), "b?")]
    [InlineData(typeof(byte), "y")]
    [InlineData(typeof(byte?), "y?")]
    [InlineData(typeof(char), "c")]
    [InlineData(typeof(char?), "c?")]
    [InlineData(typeof(DateTimeOffset), "dt")]
    [InlineData(typeof(DateTimeOffset?), "dt?")]
    [InlineData(typeof(Guid), "id")]
    [InlineData(typeof(Guid?), "id?")]
    [InlineData(typeof(int), "i")]
    [InlineData(typeof(int?), "i?")]
    [InlineData(typeof(long), "l")]
    [InlineData(typeof(long?), "l?")]
    [InlineData(typeof(string), "s")]
    [InlineData(typeof(TimeSpan), "ts")]
    [InlineData(typeof(TimeSpan?), "ts?")]
    [InlineData(typeof(Array), "ar")]
    [InlineData(typeof(ImmutableArray<int>), "imar<i>")]
    [InlineData(typeof(ImmutableArray<int?>), "imar<i?>")]
    [InlineData(typeof(ImmutableArray<int>?), "imar<i>?")]
    [InlineData(typeof(ImmutableArray<int?>?), "imar<i?>?")]
    [InlineData(typeof(ImmutableList<int>), "imli<i>")]
    [InlineData(typeof(ImmutableList<int?>), "imli<i?>")]
    [InlineData(typeof(ImmutableSortedSet<int>), "imss<i>")]
    [InlineData(typeof(ImmutableSortedSet<int?>), "imss<i?>")]
    [InlineData(typeof(ImmutableDictionary<int, string>), "imdi<i,s>")]
    [InlineData(typeof(ImmutableSortedDictionary<int, string>), "imsd<i,s>")]
    [InlineData(typeof(Version1_ModelRecord), "JSSoft_Modelora_Tests_ModelRecord")]
    [InlineData(typeof(ModelRecord), "JSSoft_Modelora_Tests_ModelRecord")]
    public void GetTypeName(Type type, string expectedName)
    {
        var actualName = TypeUtility.GetTypeName(type);
        Assert.Equal(expectedName, actualName);
    }

    [Theory]
    [InlineData("bi", typeof(BigInteger))]
    [InlineData("bi?", typeof(BigInteger?))]
    [InlineData("b", typeof(bool))]
    [InlineData("b?", typeof(bool?))]
    [InlineData("y", typeof(byte))]
    [InlineData("y?", typeof(byte?))]
    [InlineData("c", typeof(char))]
    [InlineData("c?", typeof(char?))]
    [InlineData("dt", typeof(DateTimeOffset))]
    [InlineData("dt?", typeof(DateTimeOffset?))]
    [InlineData("id", typeof(Guid))]
    [InlineData("id?", typeof(Guid?))]
    [InlineData("i", typeof(int))]
    [InlineData("i?", typeof(int?))]
    [InlineData("l", typeof(long))]
    [InlineData("l?", typeof(long?))]
    [InlineData("s", typeof(string))]
    [InlineData("ts", typeof(TimeSpan))]
    [InlineData("ts?", typeof(TimeSpan?))]
    [InlineData("ar", typeof(Array))]
    [InlineData("i[]", typeof(int[]))]
    [InlineData("i?[]", typeof(int?[]))]
    [InlineData("imar<i>", typeof(ImmutableArray<int>))]
    [InlineData("imar<i?>", typeof(ImmutableArray<int?>))]
    [InlineData("imar<i>?", typeof(ImmutableArray<int>?))]
    [InlineData("imar<i?>?", typeof(ImmutableArray<int?>?))]
    [InlineData("imli<i>", typeof(ImmutableList<int>))]
    [InlineData("imli<i?>", typeof(ImmutableList<int?>))]
    [InlineData("imss<i>", typeof(ImmutableSortedSet<int>))]
    [InlineData("imss<i?>", typeof(ImmutableSortedSet<int?>))]
    [InlineData("imdi<i,s>", typeof(ImmutableDictionary<int, string>))]
    [InlineData("imsd<i,s>", typeof(ImmutableSortedDictionary<int, string>))]
    [InlineData("JSSoft_Modelora_Tests_ModelRecord", typeof(ModelRecord))]
    public void GetType_Test(string typeName, Type expectedType)
    {
        var actualType = TypeUtility.GetType(typeName);
        Assert.Equal(expectedType, actualType);
    }

    [Theory]
    [InlineData("invalid", typeof(KeyNotFoundException))]
    [InlineData("invalid<", typeof(ArgumentException))]
    [InlineData("invalid[", typeof(ArgumentException))]
    [InlineData("[1,]", typeof(ArgumentException))]
    [InlineData("[, ]i", typeof(ArgumentException))]
    [InlineData("imar<?i>", typeof(ArgumentException))]
    [InlineData("imar <i>", typeof(ArgumentException))]
    [InlineData("imar < i >", typeof(ArgumentException))]
    [InlineData("imar <i >", typeof(ArgumentException))]
    [InlineData("imar < i>", typeof(ArgumentException))]
    [InlineData("i??", typeof(ArgumentException))]
    public void GetType_Throw(string typeName, Type expectedExceptionType)
    {
        var ex = Record.Exception(() => TypeUtility.GetType(typeName));
        Assert.NotNull(ex);
        Assert.IsType(expectedExceptionType, ex);
    }

    [Theory]
    [InlineData(typeof(int), 0)]
    [InlineData(typeof(bool), false)]
    public void CreateInstance_ValueTypes_Success(Type type, object expected)
    {
        var obj = TypeUtility.CreateInstance(type);
        Assert.Equal(expected, obj);
        Assert.Equal(type, obj.GetType());
    }

    [Fact]
    public void CreateInstance_Guid_Success()
    {
        var obj = TypeUtility.CreateInstance(typeof(Guid));
        Assert.IsType<Guid>(obj);
        Assert.Equal(Guid.Empty, (Guid)obj);
    }

    [Fact]
    public void CreateInstance_ReferenceType_WithDefaultCtor_Success()
    {
        var obj = TypeUtility.CreateInstance(typeof(object));
        Assert.NotNull(obj);
        Assert.IsType<object>(obj);
    }

    [Fact]
    public void CreateInstance_WithArguments_Success()
    {
        var obj = (HasArgs)TypeUtility.CreateInstance(typeof(HasArgs), 42, "hello");
        Assert.Equal(42, obj.X);
        Assert.Equal("hello", obj.Y);
    }

    [Fact]
    public void CreateInstance_Nullable_Throws()
    {
        // Activator.CreateInstance(typeof(Nullable<int>)) yields null; TypeUtility wraps this as ModelCreationException.
        var ex = Assert.Throws<ModelCreationException>(() => TypeUtility.CreateInstance(typeof(int?)));
        Assert.Null(ex.InnerException); // no inner exception for the null case
    }

    [Fact]
    public void CreateInstance_ConstructorThrows_Throws()
    {
        var ex = Assert.Throws<ModelCreationException>(() => TypeUtility.CreateInstance(typeof(CtorThrows)));
        Assert.NotNull(ex.InnerException);
    }

    public sealed class GetDefaultCasesData : TheoryData<Type, object>
    {
        public GetDefaultCasesData()
        {
            Add(typeof(int), 0);
            Add(typeof(int?), 0);
            Add(typeof(long), 0L);
            Add(typeof(long?), 0L);
            Add(typeof(bool), false);
            Add(typeof(bool?), false);
            Add(typeof(Guid), default(Guid));
            Add(typeof(Guid?), default(Guid));
            Add(typeof(DateTimeOffset), default(DateTimeOffset));
            Add(typeof(DateTimeOffset?), default(DateTimeOffset));
            Add(typeof(TimeSpan), default(TimeSpan));
            Add(typeof(TimeSpan?), default(TimeSpan));
            Add(typeof(BigInteger), default(BigInteger));
            Add(typeof(BigInteger?), default(BigInteger));
            Add(typeof(string), string.Empty);
        }
    }

    [Theory]
    [ClassData(typeof(GetDefaultCasesData))]
    public void GetDefault(Type type, object expected)
    {
        var actual = TypeUtility.GetDefault(type);
        Assert.Equal(expected, actual);

        if (Nullable.GetUnderlyingType(type) is { } underlying)
        {
            Assert.Equal(underlying, actual.GetType());
        }
    }

    [Theory]
    [ClassData(typeof(GetDefaultCasesData))]
    public void TryGetDefault_Success(Type type, object expected)
    {
        var result = TypeUtility.TryGetDefault(type, out var value);
        Assert.True(result);
        Assert.NotNull(value);
        Assert.Equal(expected, value);

        if (Nullable.GetUnderlyingType(type) is { } underlying)
        {
            Assert.Equal(underlying, value!.GetType());
        }
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(Dictionary<int, int>))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(Array))]
    [InlineData(typeof(DayOfWeek))]
    [InlineData(typeof(ConsoleColor?))]
    public void TryGetDefault_False(Type type)
    {
        var result = TypeUtility.TryGetDefault(type, out var value);
        Assert.False(result);
        Assert.Null(value);
    }

    [Theory]
    [InlineData(typeof(object))]
    [InlineData(typeof(List<int>))]
    [InlineData(typeof(Dictionary<int, int>))]
    [InlineData(typeof(int[]))]
    [InlineData(typeof(Array))]
    [InlineData(typeof(DayOfWeek))]
    [InlineData(typeof(ConsoleColor?))]
    public void GetDefault_Throw(Type type)
    {
        var ex = Assert.Throws<ArgumentException>(() => TypeUtility.GetDefault(type));
        Assert.Equal("type", ex.ParamName);
    }
}
