// <copyright file="ModelTypeScope.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

#if MODEL_JSON
namespace JSSoft.Modelora.Json;
#elif MODEL_YAML
namespace JSSoft.Modelora.Yaml;
#else
#error This file must be included in either JSSoft.Modelora.Json or JSSoft.Modelora.Yaml project.
#endif

internal static class ModelTypeScope
{
    private static readonly ThreadLocal<Stack<Type>?> _stack = new();

    public static Type Current => _stack.Value is { Count: > 0 } s ? s.Peek() : typeof(object);

    public static IDisposable Push(Type type)
    {
        var s = _stack.Value ??= new Stack<Type>();
        s.Push(type);
        return new PopOnDispose(s);
    }

    public static bool CanOmitTypeInfo(Type type) => CanOmitTypeInfo(type, ModelOptionsScope.Current);

    public static bool CanOmitTypeInfo(Type type, ModelOptions options)
    {
        if (options.TypeInfoEmission is TypeInfoEmission.Always)
        {
            return false;
        }

        if (options.TypeInfoEmission is TypeInfoEmission.Never)
        {
            return true;
        }

        return Current.IsSealed && type == Current;
    }

    private sealed class PopOnDispose(Stack<Type> stack) : IDisposable
    {
        public void Dispose()
        {
            if (stack is { Count: > 0 })
            {
                stack.Pop();
            }
        }
    }
}
