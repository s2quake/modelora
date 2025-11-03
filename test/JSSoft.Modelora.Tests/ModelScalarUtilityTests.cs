// <copyright file="ModelScalarUtilityTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;

namespace JSSoft.Modelora.Tests;

public sealed class ModelScalarUtilityTests
{
    [Fact]
    public void GetScalarKind_ReturnsExpected()
    {
        Assert.Equal(ModelScalarKind.Int32, ModelScalarUtility.GetScalarKind(typeof(Int32ScalarValue)));
        Assert.Equal(ModelScalarKind.Int64, ModelScalarUtility.GetScalarKind(typeof(Int64ScalarValue)));
        Assert.Equal(ModelScalarKind.String, ModelScalarUtility.GetScalarKind(typeof(StringScalarValue)));
        Assert.Equal(ModelScalarKind.Boolean, ModelScalarUtility.GetScalarKind(typeof(BooleanScalarValue)));
        Assert.Equal(ModelScalarKind.Hex, ModelScalarUtility.GetScalarKind(typeof(HexScalarValue)));
        Assert.Equal(ModelScalarKind.Hex, ModelScalarUtility.GetScalarKind(typeof(HexScalarValue<int>)));
        Assert.Equal(ModelScalarKind.Hex, ModelScalarUtility.GetScalarKind(typeof(HexScalarValue<string>)));
    }

    [Fact]
    public void GetScalarValue_ReturnsExpected_ForAllKinds()
    {
        var i32 = new Int32ScalarValue(123);
        var i64 = new Int64ScalarValue(45678901234);
        var str = new StringScalarValue("hello");
        var bln = new BooleanScalarValue(true);
        var bytes = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
        var hex = new HexScalarValue(bytes);
        var hexI32 = new HexScalarValue<int>(42);
        var hexStr = new HexScalarValue<string>("world");

        Assert.Equal(123, ModelScalarUtility.GetScalarValue(i32));
        Assert.Equal(45678901234L, ModelScalarUtility.GetScalarValue(i64));
        Assert.Equal("hello", ModelScalarUtility.GetScalarValue(str));
        Assert.True((bool)ModelScalarUtility.GetScalarValue(bln));
        Assert.Equal(bytes, (byte[])ModelScalarUtility.GetScalarValue(hex));
        Assert.IsType<byte[]>(ModelScalarUtility.GetScalarValue(hexI32));
        Assert.IsType<byte[]>(ModelScalarUtility.GetScalarValue(hexStr));
    }

    [Fact]
    public void GetObjectFromScalarValue_RoundTrips_ForAllKinds()
    {
        var i32 = (Int32ScalarValue)ModelScalarUtility.GetObjectFromScalarValue(typeof(Int32ScalarValue), 321);
        var i64 = (Int64ScalarValue)ModelScalarUtility.GetObjectFromScalarValue(typeof(Int64ScalarValue), 9876543210L);
        var str = (StringScalarValue)ModelScalarUtility.GetObjectFromScalarValue(typeof(StringScalarValue), "hi");
        var bln = (BooleanScalarValue)ModelScalarUtility.GetObjectFromScalarValue(typeof(BooleanScalarValue), false);
        var bytes = new byte[] { 1, 2, 3 };
        var hex = (HexScalarValue)ModelScalarUtility.GetObjectFromScalarValue(typeof(HexScalarValue), bytes);
        var hexI32 = (HexScalarValue<int>)ModelScalarUtility.GetObjectFromScalarValue(typeof(HexScalarValue<int>), new byte[] { 0, 0, 0, 0x2A });
        var hexStr = (HexScalarValue<string>)ModelScalarUtility.GetObjectFromScalarValue(typeof(HexScalarValue<string>), System.Text.Encoding.UTF8.GetBytes("ok"));

        Assert.Equal(new Int32ScalarValue(321), i32);
        Assert.Equal(new Int64ScalarValue(9876543210L), i64);
        Assert.Equal(new StringScalarValue("hi"), str);
        Assert.Equal(new BooleanScalarValue(false), bln);
        Assert.Equal(bytes, hex.ToScalarValue());
        Assert.IsType<HexScalarValue<int>>(hexI32);
        Assert.IsType<HexScalarValue<string>>(hexStr);
    }

    [Fact]
    public void NonPublic_Methods_Are_Supported()
    {
        var obj = NonPublicScalar.Create(42);
        Assert.Equal(42, ModelScalarUtility.GetScalarValue(obj));

        var obj2 = (NonPublicScalar)ModelScalarUtility.GetObjectFromScalarValue(typeof(NonPublicScalar), 7);
        Assert.Equal(7, ModelScalarUtility.GetScalarValue(obj2));
    }

    [Fact]
    public void GetScalarKind_NoAttribute_Throws()
    {
        Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetScalarKind(typeof(object)));
    }

    [Fact]
    public void GetScalarValue_NoAttribute_Throws()
    {
        Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetScalarValue(new object()));
    }

    [Fact]
    public void GetScalarValue_ReturnsNull_Throws_InvalidOperation()
    {
        var obj = new NullReturningString("x");
        Assert.Throws<InvalidOperationException>(() => ModelScalarUtility.GetScalarValue(obj));
    }

    [Fact]
    public void GetObjectFromScalarValue_NoAttribute_Throws()
    {
        Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(object), 1));
    }

    [Fact]
    public void GetObjectFromScalarValue_ReturnsNull_Throws_InvalidOperation()
    {
        Assert.Throws<InvalidOperationException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(NullFromScalar), "x"));
    }

    [Fact]
    public void GetScalarValue_Unwraps_Inner_Exception()
    {
        var obj = new ThrowingToScalar();
        var ex = Assert.Throws<NotSupportedException>(() => ModelScalarUtility.GetScalarValue(obj));
        Assert.Equal("boom", ex.Message);
    }

    [Fact]
    public void GetObjectFromScalarValue_Unwraps_Inner_Exception()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(ThrowingFromScalar), "x"));
        Assert.Equal("fail", ex.Message);
    }

    // Ensure analyzers see reflective members as used
    static ModelScalarUtilityTests() => TouchMembers();

    private static void TouchMembers()
    {
        var sp = new DummyServiceProvider();
        var n = NonPublicScalar.Create(0);
        _ = n.ToScalarValue();
        _ = NonPublicScalar.FromScalarValue(sp, 0);

        var nr = new NullReturningString("x");
        _ = nr.ToScalarValue();
        _ = NullReturningString.FromScalarValue(sp, "y");

        var nf = new NullFromScalar();
        _ = nf.ToScalarValue();
        _ = NullFromScalar.FromScalarValue(sp, "z");

        var tt = new ThrowingToScalar();
        try
        {
            _ = tt.ToScalarValue();
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }

        try
        {
            _ = ThrowingToScalar.FromScalarValue(sp, "t");
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }

        var tf = new ThrowingFromScalar();
        _ = tf.ToScalarValue();
        try
        {
            _ = ThrowingFromScalar.FromScalarValue(sp, "f");
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }

    // Helper/test-only types

    [ModelScalar("non_public", Kind = ModelScalarKind.Int32)]
    private sealed class NonPublicScalar
    {
        private readonly int _v;

        private NonPublicScalar(int v) => _v = v;

        public static NonPublicScalar Create(int v) => new(v);

        internal int ToScalarValue() => _v;

        internal static NonPublicScalar FromScalarValue(IServiceProvider serviceProvider, int value)
        {
            _ = serviceProvider;
            return new(value);
        }
    }

    [ModelScalar("null_returning_str", Kind = ModelScalarKind.String)]
    private sealed class NullReturningString
    {
        public NullReturningString(string value) => Value = value;

        public string Value { get; }

        public string? ToScalarValue()
        {
            _ = Value;
            return null;
        }

        public static NullReturningString FromScalarValue(IServiceProvider serviceProvider, string value)
        {
            _ = serviceProvider;
            return new(value);
        }
    }

    [ModelScalar("null_from_scalar", Kind = ModelScalarKind.String)]
    private sealed class NullFromScalar
    {
        private readonly string _v = string.Empty;

        public string ToScalarValue() => _v;

        public static NullFromScalar? FromScalarValue(IServiceProvider serviceProvider, string value)
        {
            _ = serviceProvider;
            _ = value;
            return null;
        }
    }

    [ModelScalar("throwing_to_scalar", Kind = ModelScalarKind.String)]
    private sealed class ThrowingToScalar
    {
        public string ToScalarValue()
        {
            _ = this;
            throw new NotSupportedException("boom");
        }

        public static ThrowingToScalar FromScalarValue(IServiceProvider serviceProvider, string value)
        {
            _ = serviceProvider;
            _ = value;
            return new();
        }
    }

    [ModelScalar("throwing_from_scalar", Kind = ModelScalarKind.String)]
    private sealed class ThrowingFromScalar
    {
        public string ToScalarValue() => ToString() ?? string.Empty;

        public static ThrowingFromScalar FromScalarValue(IServiceProvider serviceProvider, string value)
        {
            _ = serviceProvider;
            _ = value;
            throw new InvalidOperationException("fail");
        }
    }

    private sealed class DummyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
