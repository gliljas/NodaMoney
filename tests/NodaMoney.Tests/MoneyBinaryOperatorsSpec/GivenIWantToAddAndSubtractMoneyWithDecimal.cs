using System.Collections.Generic;
using FluentAssertions;
using NodaMoney.Tests.Helpers;
using Xunit;

namespace NodaMoney.Tests.MoneyBinaryOperatorsSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToAddAndSubtractMoneyWithDecimal
{
    public static IEnumerable<object[]> TestData => new[]
    {
        new object[] { 101m, 99m, 200m }, // whole numbers
        new object[] { 100m, 0.01m, 100.01m }, // fractions
        new object[] { 100.999m, 0.9m, 101.899m }, // overflow
        new object[] { 100.5m, 0.9m, 101.4m }, // overflow
        new object[] { 100.999m, -0.9m, 100.099m }, // negative
        new object[] { -100.999m, -0.9m, -101.899m } // negative
    };

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenUsingAdditionOperator_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");

        Money result1 = money1 + value2;
        Money result2 = value2 + money1;

        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money1);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenUsingAdditionMethod_ThenMoneyShouldBeAdded(decimal value1, decimal value2, decimal expected)
    {
        var money1 = new Money(value1, "EUR");

        var result = Money.Add(money1, value2);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenUsingSubtractionOperator_ThenMoneyShouldBeAdded(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1, "EUR");

        Money result1 = money1 - value2;
        Money result2 = value2 - money1;

        result1.Should().Be(new Money(expected, "EUR"));
        result1.Should().NotBeSameAs(money1);
        result2.Should().Be(new Money(expected, "EUR"));
        result2.Should().NotBeSameAs(money1);
    }

    [Theory, MemberData(nameof(TestData))]
    [UseCulture("en-us")]
    public void WhenUsingSubtractionMethod_ThenMoneyShouldBeSubtracted(decimal expected, decimal value2, decimal value1)
    {
        var money1 = new Money(value1, "EUR");

        var result = Money.Subtract(money1, value2);

        result.Should().Be(new Money(expected, "EUR"));
        result.Should().NotBeSameAs(money1);
    }
}
