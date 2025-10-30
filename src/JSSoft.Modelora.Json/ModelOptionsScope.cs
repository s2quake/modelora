// <copyright file="ModelOptionsScope.cs" company="JSSoft">
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

internal static class ModelOptionsScope
{
    private static readonly ThreadLocal<Stack<ModelOptions>?> _stack = new();

    public static ModelOptions Current => _stack.Value is { Count: > 0 } s ? s.Peek() : ModelOptions.Empty;

    public static IDisposable Push(ModelOptions options)
    {
        var s = _stack.Value ??= new Stack<ModelOptions>();
        s.Push(options);
        return new PopOnDispose(s);
    }

    private sealed class PopOnDispose(Stack<ModelOptions> s) : IDisposable
    {
        public void Dispose()
        {
            if (s is { Count: > 0 })
            {
                s.Pop();
            }
        }
    }
}
