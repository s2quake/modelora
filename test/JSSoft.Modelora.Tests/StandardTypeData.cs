// <copyright file="StandardTypeData.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;

namespace JSSoft.Modelora.Tests;

public sealed class StandardTypeData : IEnumerable<TheoryDataRow<object>>
{
    public IEnumerator<TheoryDataRow<object>> GetEnumerator()
    {
        yield return new TheoryDataRow<object>((BigInteger)0);
        yield return new TheoryDataRow<object>((BigInteger)1);
        yield return new TheoryDataRow<object>(true);
        yield return new TheoryDataRow<object>(false);
        yield return new TheoryDataRow<object>(Array.Empty<byte>());
        yield return new TheoryDataRow<object>(new byte[] { 0, 1, 2, 3 });
        yield return new TheoryDataRow<object>(DateTimeOffset.MinValue);
        yield return new TheoryDataRow<object>(DateTimeOffset.MaxValue);
        yield return new TheoryDataRow<object>(ImmutableArray<byte>.Empty);
        yield return new TheoryDataRow<object>(ImmutableArray.Create<byte>(0, 1, 2, 3));
        yield return new TheoryDataRow<object>(0);
        yield return new TheoryDataRow<object>(1);
        yield return new TheoryDataRow<object>(0L);
        yield return new TheoryDataRow<object>(1L);
        yield return new TheoryDataRow<object>(string.Empty);
        yield return new TheoryDataRow<object>("Hello, World!");
        yield return new TheoryDataRow<object>(TimeSpan.Zero);
        yield return new TheoryDataRow<object>(TimeSpan.FromSeconds(1));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
