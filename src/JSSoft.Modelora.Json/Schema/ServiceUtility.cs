// <copyright file="ServiceUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;

namespace JSSoft.Modelora.Json.Schema;

internal static class ServiceUtility
{
    public static IEnumerable<Type> GetTypes(Assembly assembly, Type attributeType, bool inherit)
    {
        try
        {
            return assembly.GetTypes()
            .Where(type => Attribute.IsDefined(type, attributeType, inherit))
            .Where(type => type.IsClass && !type.IsAbstract);
        }
        catch (ReflectionTypeLoadException)
        {
            return [];
        }
    }

    public static IEnumerable<Type> GetTypes(Type attributeType, bool inherit)
        => AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => GetTypes(assembly, attributeType, inherit));
}
