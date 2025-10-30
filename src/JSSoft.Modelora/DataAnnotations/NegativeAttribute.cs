// <copyright file="NegativeAttribute.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.DataAnnotations;

[AttributeUsage(AttributeTargets.Property)]
public sealed class NegativeAttribute : SignComparisonAttribute
{
    protected override bool Compare(IComparable value, IComparable target) => value.CompareTo(target) < 0;

    protected override string FormatErrorMessage(
        string memberName, Type declaringType, IComparable value, IComparable target)
        => $"The property {memberName} of {declaringType.Name} must be less than zero. Current value: {value}.";
}
