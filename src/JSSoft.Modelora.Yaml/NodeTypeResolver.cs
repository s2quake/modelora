// <copyright file="NodeTypeResolver.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml;

internal sealed class NodeTypeResolver : INodeTypeResolver
{
    public bool Resolve(NodeEvent? nodeEvent, ref Type currentType)
    {
        if (nodeEvent is Scalar { Style: ScalarStyle.Plain, Value: "null" })
        {
            return false;
        }

        if (currentType != typeof(object))
        {
            return false;
        }

        return true;
    }
}
