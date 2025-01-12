﻿using System;
using FluentAssertions;
using Xunit;
using NodaMoney.Tests.Helpers;

namespace NodaMoney.Tests.CurrencyInfoBuilderSpec;

[Collection(nameof(NoParallelization))]
public class GivenIWantToCreateCustomCurrencyInfo
{
    [Fact]
    public void WhenRegisterWithDefaults_ThenShouldBeAvailableWithDefaults()
    {
        // Arrange
        CurrencyInfoBuilder builder = new("BTA");

        // Act
        CurrencyInfo result = builder.Register();

        // Assert
        result.Code.Should().Be("BTA");
        result.IsIso4217.Should().BeFalse();
        result.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign);
        result.EnglishName.Should().BeEmpty();
        result.NumericCode.Should().Be("000");
        result.DecimalDigits.Should().Be(0);

        CurrencyInfo ci = CurrencyInfo.FromCode("BTA");
        ci.Should().BeEquivalentTo(result);
        ci.Should().Be(result);
    }

    [Fact]
    public void WhenRegisterInIsoNamespace_ThenShouldBeAvailable()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("BTB")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8,
            IsIso4217 = true // in iso namespace
        };

        // Act
        CurrencyInfo result = builder.Register();

        // Assert
        result.Code.Should().Be("BTB");
        result.IsIso4217.Should().BeTrue();
        result.Symbol.Should().Be("฿");
        result.EnglishName.Should().Be("Bitcoin");
        result.NumericCode.Should().Be("123");
        result.DecimalDigits.Should().Be(8);

        CurrencyInfo ci = CurrencyInfo.FromCode("BTB");
        ci.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenRegisterBitCoin_ThenShouldBeAvailableByExplicitNamespace()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("BTE")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8,
            IsIso4217 = false, // not in iso namespace
        };

        // Act
        CurrencyInfo result = builder.Register();

        // Assert
        CurrencyInfo bitcoinCI = CurrencyInfo.FromCode("BTE");
        bitcoinCI.Symbol.Should().Be("฿");
        bitcoinCI.Should().BeEquivalentTo(result);
    }

    [Fact]
    public void WhenBuildBitCoin_ThenItShouldSucceedButNotBeRegistered()
    {
        // Arrange
        var builder = new CurrencyInfoBuilder("BTD")
        {
            EnglishName = "Bitcoin",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8,
            IsIso4217 = false
        };

        // Act
        CurrencyInfo result = builder.Build();

        // Assert
        result.Symbol.Should().Be("฿");

        //Action action = () => Currency.FromCode("BTD", "virtual");
        Action action = () => Currency.FromCode("BTD");
        action.Should().Throw<InvalidCurrencyException>();//.WithMessage("BTD is unknown currency code!");
    }

    [Fact]
    public void WhenFromExistingCurrency_ThenThisShouldSucceed()
    {
        var builder = new CurrencyInfoBuilder("BTE") { IsIso4217 = false };

        var euro = CurrencyInfo.FromCode("EUR");
        builder.LoadDataFromCurrencyInfo(euro);

        var euroInfo = CurrencyInfo.FromCode("EUR");
        builder.Code.Should().Be("BTE");
        builder.IsIso4217.Should().BeTrue();
        builder.EnglishName.Should().Be(euroInfo.EnglishName);
        builder.Symbol.Should().Be(euro.Symbol);
        builder.NumericCode.Should().Be(euroInfo.NumericCode);
        builder.DecimalDigits.Should().Be(euroInfo.DecimalDigits);
        builder.ValidFrom.Should().Be(euroInfo.IntroducedOn);
        builder.ValidTo.Should().Be(euroInfo.ExpiredOn);
    }

    [Fact]
    public void WhenRegisterExistingCurrency_ThenThrowException()
    {
        var builder = new CurrencyInfoBuilder("EUR");

        var euro = CurrencyInfo.FromCode("EUR");
        builder.LoadDataFromCurrencyInfo(euro);

        Action action = () => builder.Register();

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is already registered*");
    }

    [Fact]
    public void WhenCodeIsNull_ThenThrowException()
    {
        Action action = () => new CurrencyInfoBuilder(null) { IsIso4217 = false };

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThenThrowException()
    {
        Action action = () => new CurrencyInfoBuilder("")  { IsIso4217 = false };

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenSymbolIsEmpty_ThenSymbolMustBeDefaultCurrencySign()
    {
        Currency result = new CurrencyInfoBuilder("BTH")
        {
            EnglishName = "Bitcoin",
            //Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 8,
        }.Register();

        Currency bitcoin = Currency.FromCode("BTH");
        bitcoin.Symbol.Should().Be(CurrencyInfo.GenericCurrencySign); // ¤
        bitcoin.Should().BeEquivalentTo(result);
    }
}

[Collection(nameof(NoParallelization))]
public class GivenIWantToUnregisterCurrency
{
    [Fact]
    public void WhenUnregisterIsoCurrency_ThenThisMustSucceed()
    {
        var money = Currency.FromCode("PEN"); // should work

        CurrencyInfoBuilder.Unregister("PEN");
        Action action = () => Currency.FromCode("PEN");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenUnregisterCustomCurrency_ThenThisMustSucceed()
    {
        var builder = new CurrencyInfoBuilder("XYZ")
        {
            EnglishName = "Xyz",
            Symbol = "฿",
            NumericCode = "123", // iso number
            DecimalDigits = 4,
            IsIso4217 = false
        };

        builder.Register();
        //Currency xyz = Currency.FromCode("XYZ", "virtual"); // should work
        Currency xyz = Currency.FromCode("XYZ"); // should work

        CurrencyInfoBuilder.Unregister("XYZ");
        //Action action = () => Currency.FromCode("XYZ", "virtual");
        Action action = () => Currency.FromCode("XYZ");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*unknown*currency*");
    }

    [Fact]
    public void WhenCurrencyDoesNotExist_ThenThrowException()
    {
        Action action = () => CurrencyInfoBuilder.Unregister("ABC");

        action.Should().Throw<InvalidCurrencyException>().WithMessage("*is unknown currency code!");
    }

    [Fact]
    public void WhenCodeIsNull_ThenThrowException()
    {
        Action action = () => CurrencyInfoBuilder.Unregister(null);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void WhenCodeIsEmpty_ThenThrowException()
    {
        Action action = () => CurrencyInfoBuilder.Unregister("");

        action.Should().Throw<ArgumentNullException>();
    }
}

[Collection(nameof(NoParallelization))]
public class GivenIWantToReplaceIsoCurrencyWithOwnVersion
{
    [Fact]
    public void WhenReplacingEuroWithCustom_ThenThisShouldSucceed()
    {
        // Panamanian balboa
        CurrencyInfo oldEuro = CurrencyInfoBuilder.Unregister("PAB");

        var builder = new CurrencyInfoBuilder("PAB");
        builder.LoadDataFromCurrencyInfo(oldEuro);
        builder.EnglishName = "New Panamanian balboa";
        builder.DecimalDigits = 1;

        builder.Register();

        CurrencyInfo newEuro = CurrencyInfo.FromCode("PAB");
        newEuro.Symbol.Should().Be("B/.");
        newEuro.EnglishName.Should().Be("New Panamanian balboa");
        newEuro.DecimalDigits.Should().Be(1);
    }
}
