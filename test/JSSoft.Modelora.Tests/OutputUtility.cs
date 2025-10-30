// <copyright file="OutputUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using static JSSoft.Randora.RandomUtility;

namespace JSSoft.Modelora.Tests;

public static class OutputUtility
{
    public static Random GetRandom(ITestOutputHelper output)
    {
        var seed = Int32();
        output.WriteLine($"Random seed: {seed}");
        return new Random(seed);
    }

    public static Random GetRandom(ITestOutputHelper output, int seed)
    {
        output.WriteLine($"Random seed: {seed}");
        return new Random(seed);
    }

    public static Random GetStaticRandom(ITestOutputHelper output)
    {
        var seed = 0;
        output.WriteLine($"Random seed: {seed}");
        return new Random(seed);
    }
}
