namespace NodaMoney.Tests.MoneyConvertibleSpec;

public class MoneyOACurrency
{
    [Fact]
    public void ToOACurrency_ShouldConvertValidAmountToOACurrency()
    {
        // Arrange
        var money = new Money(123.4567m, CurrencyInfo.FromCode("USD"));

        // Act
        var oaCurrencyValue = money.ToOACurrency();

        // Assert
        oaCurrencyValue.Should().Be(1234600); // Money type will round 123.4567 to USD 123.46
    }

    [Fact]
    public void ToOACurrency_ShouldThrowExceptionForCurrenciesWithMoreThan4DecimalPlaces()
    {
        // Arrange
        var money = new Money(0.123456m, CurrencyInfo.FromCode("BTC"));

        // Act
        Action act = () => money.ToOACurrency();

        // Assert
        act.Should().Throw<InvalidCurrencyException>()
           .WithMessage("The currency 'BTC' requires more than 4 decimal places, which cannot be represented by OLE Automation Currency.");
    }

    [Fact]
    public void FromOACurrency_ShouldConvertValidOACurrencyValueToMoney()
    {
        // Arrange
        Currency currency = CurrencyInfo.FromCode("EUR");
        long oaCurrencyValue = 1234567;

        // Act
        var money = Money.FromOACurrency(oaCurrencyValue, currency);

        // Assert
        money.Amount.Should().Be(123.4600m); // Money type will round 123.4567 to USD 123.46
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void FromOACurrency_ShouldThrowExceptionForCurrenciesWithMoreThan4DecimalPlaces()
    {
        // Arrange
        var currency = CurrencyInfo.FromCode("BTC");
        long oaCurrencyValue = 123456789;

        // Act
        Action act = () => Money.FromOACurrency(oaCurrencyValue, currency);

        // Assert
        act.Should().Throw<InvalidCurrencyException>()
           .WithMessage("The currency 'BTC' requires more than 4 decimal places, which cannot be represented by OLE Automation Currency.");
    }

    [Fact]
    public void ToOACurrency_ShouldWorkForCurrencyWithExactly4DecimalPlaces()
    {
        // Arrange
        var currency = CurrencyInfo.FromCode("CLF"); // CLF = Unidad de Fomento (funds code) has 4 decimals
        var money = new Money(123.4567m, currency);

        // Act
        var oaCurrencyValue = money.ToOACurrency();

        // Assert
        oaCurrencyValue.Should().Be(1234567);
    }

    [Fact]
    public void FromOACurrency_ShouldWorkForCurrencyWithExactly4DecimalPlaces()
    {
        // Arrange
        Currency currency = CurrencyInfo.FromCode("CLF"); // CLF = Unidad de Fomento (funds code) has 4 decimals
        long oaCurrencyValue = 1234567;

        // Act
        var money = Money.FromOACurrency(oaCurrencyValue, currency);

        // Assert
        money.Amount.Should().Be(123.4567m);
        money.Currency.Should().Be(currency);
    }
}
