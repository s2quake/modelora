// <copyright file="ImmutableCollectionJsonConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.Reflection;

namespace JSSoft.Modelora.Json.DynamicConverters;

internal abstract class ImmutableCollectionJsonConverter : CollectionJsonConverter
{
    protected abstract Type ImmutableStaticType { get; }

    protected override IEnumerable CreateInstance(Type typeToConvert, Type elementType, IList listInstance)
    {
        var methodInfo = GetCreateRangeMethod(ImmutableStaticType, IsDictionary);
        var genericMethodInfo = IsDictionary
            ? methodInfo.MakeGenericMethod(elementType.GetGenericArguments())
            : methodInfo.MakeGenericMethod(elementType);
        var methodArgs = new object?[] { listInstance };
        return (IEnumerable)genericMethodInfo.Invoke(null, parameters: methodArgs)!;
    }

    private static MethodInfo GetCreateRangeMethod(Type type, bool isDictionary)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.Static;

        if (isDictionary)
        {
            var tKey = Type.MakeGenericMethodParameter(0);
            var tValue = Type.MakeGenericMethodParameter(1);
            var kvpType = typeof(KeyValuePair<,>).MakeGenericType(tKey, tValue);
            var paramType = typeof(IEnumerable<>).MakeGenericType(kvpType);
            var mi = type.GetMethod("CreateRange", bindingFlags, binder: null, types: new[] { paramType }, modifiers: null);
            if (mi is not null)
            {
                return mi;
            }
        }
        else
        {
            var t = Type.MakeGenericMethodParameter(0);
            var paramType = typeof(IEnumerable<>).MakeGenericType(t);
            var mi = type.GetMethod("CreateRange", bindingFlags, binder: null, types: new[] { paramType }, modifiers: null);
            if (mi is not null)
            {
                return mi;
            }
        }

        throw new NotSupportedException("The method is not found.");
    }
}
