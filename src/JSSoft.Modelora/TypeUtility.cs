// <copyright file="TypeUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JSSoft.Modelora;

public static class TypeUtility
{
    private const string TypeExpression = @"(?<type>[a-zA-Z][a-zA-Z0-9_]*)";
    private const string ArrayExpression = @"(?<array_nullable>\??)(?<array>\[,*\])";
    private const string GenericExpression = @"(?<generic>\<.+\>)";
    private const string NullableExpression = @"(?<nullable>\??)";
    private static readonly string _typeExpression
        = $"^{TypeExpression}(?:{ArrayExpression}|{GenericExpression})?{NullableExpression}$";

    private static readonly ConcurrentDictionary<string, Type> _typeByFullName = [];
    private static readonly ConcurrentDictionary<Type, object> _defaultByType = [];
    private static readonly KnownTypes _knownTypes = new();
    private static readonly HashSet<Assembly> _addedAssemblies = [];
    private static readonly object _lock = new();

    static TypeUtility()
    {
        _knownTypes.AddType(typeof(object), "obj");
        _knownTypes.AddType(typeof(BigInteger), "bi");
        _knownTypes.AddType(typeof(bool), "b");
        _knownTypes.AddType(typeof(byte), "y");
        _knownTypes.AddType(typeof(char), "c");
        _knownTypes.AddType(typeof(DateTimeOffset), "dt");
        _knownTypes.AddType(typeof(Guid), "id");
        _knownTypes.AddType(typeof(int), "i");
        _knownTypes.AddType(typeof(long), "l");
        _knownTypes.AddType(typeof(string), "s");
        _knownTypes.AddType(typeof(TimeSpan), "ts");
        _knownTypes.AddType(typeof(Array), "ar");
        _knownTypes.AddType(typeof(List<>), "li<>");
        _knownTypes.AddType(typeof(HashSet<>), "hs<>");
        _knownTypes.AddType(typeof(SortedSet<>), "shs<>");
        _knownTypes.AddType(typeof(Dictionary<,>), "di<,>");
        _knownTypes.AddType(typeof(SortedDictionary<,>), "sdi<,>");
        _knownTypes.AddType(typeof(ImmutableArray<>), "imar<>");
        _knownTypes.AddType(typeof(ImmutableList<>), "imli<>");
        _knownTypes.AddType(typeof(ImmutableHashSet<>), "imhs<>");
        _knownTypes.AddType(typeof(ImmutableSortedSet<>), "imss<>");
        _knownTypes.AddType(typeof(ImmutableDictionary<,>), "imdi<,>");
        _knownTypes.AddType(typeof(ImmutableSortedDictionary<,>), "imsd<,>");
        _knownTypes.AddType(typeof(Tuple<,>), "tp<,>");
        _knownTypes.AddType(typeof(Tuple<,,>), "tp<,,>");
        _knownTypes.AddType(typeof(Tuple<,,,>), "tp<,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,>), "tp<,,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,,>), "tp<,,,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,,,>), "tp<,,,,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,,,,>), "tp<,,,,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,>), "vtp<,>");
        _knownTypes.AddType(typeof(ValueTuple<,,>), "vtp<,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,>), "vtp<,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,>), "vtp<,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,,>), "vtp<,,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,,,>), "vtp<,,,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,,,,>), "vtp<,,,,,,,>");
        _knownTypes.AddType(typeof(KeyValuePair<,>), "kvp<,>");

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            AddAssembly(assembly);
        }

        AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
        {
            if (args.LoadedAssembly is not null)
            {
                AddAssembly(args.LoadedAssembly);
            }
        };
    }

    public static Type GetType(string typeName)
    {
        if (_typeByFullName.TryGetValue(typeName, out var cachedType))
        {
            return cachedType;
        }

        var match = Regex.Match(typeName, _typeExpression);
        if (!match.Success)
        {
            throw new ArgumentException($"Invalid type name: {typeName}", nameof(typeName));
        }

        var name = match.Groups["type"].Value;
        var genericPart = Regex.Replace(match.Groups["generic"].Value, "^<(.+)>$", "$1");
        var arrayPart = match.Groups["array"].Value;

        if (genericPart != string.Empty)
        {
            var genericArgumentNames = genericPart.Contains('<') ? [genericPart] : genericPart.Split(',');
            var genericArgumentList = new List<Type>(genericArgumentNames.Length);
            var separators = string.Empty.PadRight(genericArgumentNames.Length - 1, ',');
            var typeDefinitionName = $"{name}<{separators}>";
            var typeDefinition = _knownTypes.GetType(typeDefinitionName);
            foreach (var genericArgumentName in genericArgumentNames)
            {
                var genericArgument = GetType(genericArgumentName);
                genericArgumentList.Add(genericArgument);
            }

            var type = typeDefinition.MakeGenericType([.. genericArgumentList]);
            var isNullable = match.Groups["nullable"].Value == "?";
            if (isNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(type);
                _typeByFullName[typeName] = nullableType;
                return nullableType;
            }

            _typeByFullName[typeName] = type;
            return type;
        }
        else if (arrayPart != string.Empty)
        {
            var elementTypeName = name;
            var elementType = _knownTypes.GetType(elementTypeName);
            var isNullable = match.Groups["array_nullable"].Value == "?";
            if (isNullable)
            {
                elementType = typeof(Nullable<>).MakeGenericType(elementType);
            }

            var rank = arrayPart.Count(c => c == ',') + 1;
            var type = Array.CreateInstance(elementType, new int[rank]).GetType();
            _typeByFullName[typeName] = type;
            return type;
        }
        else
        {
            var type = _knownTypes.GetType(name);
            var isNullable = match.Groups["nullable"].Value == "?";
            if (isNullable)
            {
                var nullableType = typeof(Nullable<>).MakeGenericType(type);
                _typeByFullName[typeName] = nullableType;
                return nullableType;
            }

            _typeByFullName[typeName] = type;
            return type;
        }
    }

    public static bool IsNullableType(Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static string GetTypeName(Type type)
    {
        lock (_lock)
        {
            return GetTypeNameInternal(type);
        }
    }

    public static bool IsSupportedType(Type type)
    {
        try
        {
            _ = GetTypeName(type);
            return true;
        }
        catch (NotSupportedException)
        {
            return false;
        }
    }

    public static bool HasDefaultValue(Type type)
    {
        if (type == typeof(string))
        {
            return true;
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return HasDefaultValue(underlyingType);
        }

        return type.IsValueType && !type.IsEnum;
    }

    public static bool IsDefault(object? value)
    {
        if (value is null || !HasDefaultValue(value.GetType()))
        {
            return false;
        }

        var defaultValue = _defaultByType.GetOrAdd(value.GetType(), CreateDefault);
        return ReferenceEquals(value, defaultValue) || Equals(value, defaultValue);
    }

    public static object GetDefault(Type type)
    {
        if (HasDefaultValue(type))
        {
            return _defaultByType.GetOrAdd(type, CreateDefault);
        }

        throw new ArgumentException(
            $"Type '{type.FullName}' is not a value type and does not have a default value.",
            nameof(type));
    }

    public static bool TryGetDefault(Type type, [MaybeNullWhen(false)] out object defaultValue)
    {
        if (HasDefaultValue(type))
        {
            defaultValue = _defaultByType.GetOrAdd(type, CreateDefault);
            return true;
        }

        defaultValue = null;
        return false;
    }

    public static Type GetActualType(object? value, Type type)
    {
        if (value is null)
        {
            return type;
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return typeof(Nullable<>).MakeGenericType(underlyingType);
        }

        return value.GetType();
    }

    public static object CreateInstance(Type type, params object?[] args)
    {
        try
        {
            if (Activator.CreateInstance(type, args: args) is { } obj)
            {
                return obj;
            }
        }
        catch (Exception e)
        {
            throw new ModelCreationException(type, e);
        }

        throw new ModelCreationException(type);
    }

    internal static bool IsKnownType(Type type) => _knownTypes.Contains(type);

    private static void AddAssembly(Assembly assembly)
    {
        var types = from type in assembly.GetTypes()
                    where type.IsDefined(typeof(ModelAttribute)) ||
                          type.IsDefined(typeof(ModelConverterAttribute))
                    select type;

        foreach (var type in types)
        {
            if (type.GetCustomAttribute<ModelAttribute>() is { } modelAttribute)
            {
                _knownTypes.AddType(type, modelAttribute.TypeName);
            }
            else if (type.GetCustomAttribute<ModelConverterAttribute>() is { } modelConverterAttribute)
            {
                _knownTypes.AddType(type, modelConverterAttribute.TypeName);
            }

            if (type.IsDefined(typeof(ModelScalarKnownTypeAttribute)))
            {
                var knownTypeAttributes = type.GetCustomAttributes<ModelScalarKnownTypeAttribute>();
                foreach (var knownTypeAttribute in knownTypeAttributes)
                {
                    var knownType = knownTypeAttribute.Type;
                    if (type.IsGenericType && knownType.IsGenericType && type == knownType.GetGenericTypeDefinition())
                    {
                        _knownTypes.AddType(knownTypeAttribute.Type, knownTypeAttribute.TypeName);
                    }
                    else
                    {
                        Trace.TraceError(
                            $"Type '{knownTypeAttribute.Type.FullName}' must be assignable to '{type.FullName}'.");
                    }
                }
            }
        }

        _addedAssemblies.Add(assembly);
    }

    private static object CreateDefault(Type type)
    {
        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            type = underlyingType;
        }

        if (type == typeof(string))
        {
            return string.Empty;
        }

        return Activator.CreateInstance(type)!;
    }

    private static string GetTypeNameInternal(Type type)
    {
        if (_knownTypes.TryGetTypeName(type, out var typeName))
        {
            return typeName;
        }

        if (!_addedAssemblies.Contains(type.Assembly))
        {
            AddAssembly(type.Assembly);
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            typeName = $"{GetTypeName(underlyingType)}?";
            _knownTypes.AddType(type, typeName);
            return typeName;
        }
        else if (type.IsGenericType)
        {
            var typeDefinition = type.GetGenericTypeDefinition();
            var typeDefinitionName = _knownTypes.GetTypeName(typeDefinition);
            var genericArguments = type.GetGenericArguments();
            var genericArgumentList = new List<string>(genericArguments.Length);
            foreach (var genericArgument in genericArguments)
            {
                var genericArgumentName = GetTypeName(genericArgument);
                genericArgumentList.Add(genericArgumentName);
            }

            var genericArgumentString = string.Join(',', genericArgumentList);
            typeName = Regex.Replace(typeDefinitionName, "<.*>", $"<{genericArgumentString}>");
            _knownTypes.AddType(type, typeName);
            return typeName;
        }
        else if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var elementTypeName = GetTypeName(elementType);
            var rank = type.GetArrayRank();
            typeName = $"{elementTypeName}[{new string(',', rank - 1)}]";
            _knownTypes.AddType(type, typeName);
            return typeName;
        }
        else if (type.IsDefined(typeof(OriginModelAttribute)))
        {
            var attribute = type.GetCustomAttribute<OriginModelAttribute>()!;
            return GetTypeName(attribute.Type);
        }

        throw new NotSupportedException($"Type '{type}' is not supported or not registered in known types.");
    }
}
