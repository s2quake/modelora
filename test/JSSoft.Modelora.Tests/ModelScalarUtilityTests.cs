// <copyright file="ModelScalarUtilityTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

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

    // --- Missing or invalid ToScalarValue / FromScalarValue shape tests ---

    [Fact]
    public void GetScalarValue_ToScalarValue_Missing_Throws()
    {
        var obj = new MissingToScalar();
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetScalarValue(obj));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetScalarValue_ToScalarValue_WrongReturnType_Throws()
    {
        var obj = new WrongReturnToScalar();
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetScalarValue(obj));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetScalarValue_ToScalarValue_HasParameters_Throws()
    {
        var obj = new ParameterizedToScalar();
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetScalarValue(obj));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetObjectFromScalarValue_FromScalarValue_Missing_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(MissingFromScalar), "v"));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetObjectFromScalarValue_FromScalarValue_WrongReturnType_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(WrongReturnFromScalar), "v"));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetObjectFromScalarValue_FromScalarValue_WrongParamCount_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(WrongParamCountFromScalar), "v"));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetObjectFromScalarValue_FromScalarValue_WrongFirstParamType_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(WrongFirstParamFromScalar), "v"));
        Assert.Equal("type", ex.ParamName);
    }

    [Fact]
    public void GetObjectFromScalarValue_FromScalarValue_WrongSecondParamType_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() => ModelScalarUtility.GetObjectFromScalarValue(typeof(WrongSecondParamFromScalar), "v"));
        Assert.Equal("type", ex.ParamName);
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

        // Touch invalid-shape scalar types to satisfy analyzers
        _ = MissingToScalar.FromScalarValue(sp, "m");
        var wrt = new WrongReturnToScalar();
        _ = wrt.ToScalarValue();
        _ = WrongReturnToScalar.FromScalarValue(sp, "wrt");
        var pt = new ParameterizedToScalar();
        _ = pt.ToScalarValue("p");
        _ = ParameterizedToScalar.FromScalarValue(sp, "pt");

        var mf = new MissingFromScalar();
        _ = mf.ToScalarValue();
        _ = WrongReturnFromScalar.FromScalarValue(sp, "x");
        _ = WrongParamCountFromScalar.FromScalarValue(sp, "y", 0);
        _ = WrongFirstParamFromScalar.FromScalarValue(new object(), "z");
        _ = WrongSecondParamFromScalar.FromScalarValue(sp, 1);
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

    // Types for invalid method shape scenarios

#pragma warning disable LIBP1002 // Model scalar class must have ToScalarValue method
    [ModelScalar("missing_to", Kind = ModelScalarKind.String)]
    private sealed class MissingToScalar
    {
        public static MissingToScalar FromScalarValue(IServiceProvider sp, string value)
        {
            _ = sp;
            _ = value;
            return new();
        }
    }
#pragma warning restore LIBP1002 // Model scalar class must have ToScalarValue method

#pragma warning disable LIBP1002 // Model scalar class must have ToScalarValue method
    [ModelScalar("wrong_return_to", Kind = ModelScalarKind.String)]
    private sealed class WrongReturnToScalar
    {
        // Wrong return type: int instead of string
        public int ToScalarValue()
        {
            _ = this;
            return 1;
        }

        public static WrongReturnToScalar FromScalarValue(IServiceProvider sp, string value)
        {
            _ = sp;
            _ = value;
            return new();
        }
    }
#pragma warning restore LIBP1002 // Model scalar class must have ToScalarValue method

#pragma warning disable LIBP1002 // Model scalar class must have ToScalarValue method
    [ModelScalar("parameterized_to", Kind = ModelScalarKind.String)]
    private sealed class ParameterizedToScalar
    {
        // Has a parameter - invalid
        public string ToScalarValue(string extra)
        {
            _ = this;
            _ = extra;
            return extra;
        }

        public static ParameterizedToScalar FromScalarValue(IServiceProvider sp, string value)
        {
            _ = sp;
            _ = value;
            return new();
        }
    }
#pragma warning restore LIBP1002 // Model scalar class must have ToScalarValue method

#pragma warning disable LIBP1003 // Model scalar class must have FromScalarValue method
    [ModelScalar("missing_from", Kind = ModelScalarKind.String)]
    private sealed class MissingFromScalar
    {
        public string ToScalarValue()
        {
            _ = this;
            return string.Empty;
        }
    }
#pragma warning restore LIBP1003 // Model scalar class must have FromScalarValue method

#pragma warning disable LIBP1003 // Model scalar class must have FromScalarValue method
    [ModelScalar("wrong_return_from", Kind = ModelScalarKind.String)]
    private sealed class WrongReturnFromScalar
    {
        public string ToScalarValue()
        {
            _ = this;
            return string.Empty;
        }

        // Wrong return type: string instead of the declaring type
        public static string FromScalarValue(IServiceProvider sp, string value)
        {
            _ = sp;
            return value;
        }
    }
#pragma warning restore LIBP1003 // Model scalar class must have FromScalarValue method

#pragma warning disable LIBP1003 // Model scalar class must have FromScalarValue method
    [ModelScalar("wrong_param_count_from", Kind = ModelScalarKind.String)]
    private sealed class WrongParamCountFromScalar
    {
        public string ToScalarValue()
        {
            _ = this;
            return string.Empty;
        }

        // Has 3 parameters instead of 2
        public static WrongParamCountFromScalar FromScalarValue(IServiceProvider sp, string value, int extra)
        {
            _ = sp;
            _ = value;
            _ = extra;
            return new();
        }
    }
#pragma warning restore LIBP1003 // Model scalar class must have FromScalarValue method

#pragma warning disable LIBP1003 // Model scalar class must have FromScalarValue method
    [ModelScalar("wrong_first_param_from", Kind = ModelScalarKind.String)]
    private sealed class WrongFirstParamFromScalar
    {
        public string ToScalarValue()
        {
            _ = this;
            return string.Empty;
        }

        // First parameter must be IServiceProvider
        public static WrongFirstParamFromScalar FromScalarValue(object notServiceProvider, string value)
        {
            _ = notServiceProvider;
            _ = value;
            return new();
        }
    }
#pragma warning restore LIBP1003 // Model scalar class must have FromScalarValue method

#pragma warning disable LIBP1003 // Model scalar class must have FromScalarValue method
    [ModelScalar("wrong_second_param_from", Kind = ModelScalarKind.String)]
    private sealed class WrongSecondParamFromScalar
    {
        public string ToScalarValue()
        {
            _ = this;
            return string.Empty;
        }

        // Second parameter must be string (for Kind.String)
        public static WrongSecondParamFromScalar FromScalarValue(IServiceProvider sp, int notString)
        {
            _ = sp;
            _ = notString;
            return new();
        }
    }
#pragma warning restore LIBP1003 // Model scalar class must have FromScalarValue method
}
