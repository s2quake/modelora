// <copyright file="StandardTypeDefaultData.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;

namespace JSSoft.Modelora.Tests;

public sealed class StandardTypeDefaultData : IEnumerable<TheoryDataRow<object>>
{
    public IEnumerator<TheoryDataRow<object>> GetEnumerator()
    {
        yield return new TheoryDataRow<object>(default(BigInteger));
        yield return new TheoryDataRow<object>(default(bool));
        yield return new TheoryDataRow<object>(default(byte));
        yield return new TheoryDataRow<object>(default(char));
        yield return new TheoryDataRow<object>(default(DateTimeOffset));
        yield return new TheoryDataRow<object>(default(Guid));
        yield return new TheoryDataRow<object>(default(int));
        yield return new TheoryDataRow<object>(default(long));
        yield return new TheoryDataRow<object>(default(TimeSpan));
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
