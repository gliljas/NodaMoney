﻿using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace NodaMoney.Serialization;

/// <summary>Provides a way of converting the type <see cref="string"/> to and from the type <see cref="Currency"/>.</summary>
public class CurrencyTypeConverter : TypeConverter
{
    /// <summary>Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.</summary>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    /// <param name="context">An <see cref="ITypeDescriptorContext" /> that provides a format context. </param>
    /// <param name="sourceType">A <see cref="Type" /> that represents the type you want to convert from. </param>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

    /// <summary>Returns whether this converter can convert the object to the specified type, using the specified context.</summary>
    /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
    /// <param name="context">An <see cref="ITypeDescriptorContext" /> that provides a format context. </param>
    /// <param name="destinationType">A <see cref="Type" /> that represents the type you want to convert to. </param>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(Currency) || base.CanConvertTo(context, destinationType);

    /// <summary>Converts the given object to the type of this converter, using the specified context and culture information.</summary>
    /// <returns>An <see cref="object" /> that represents the converted value.</returns>
    /// <param name="context">An <see cref="ITypeDescriptorContext" /> that provides a format context. </param>
    /// <param name="culture">The <see cref="CultureInfo" /> to use as the current culture. </param>
    /// <param name="value">The <see cref="object" /> to convert. </param>
    /// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object? value)
    {
        if (value is not string valueAsString)
            return base.ConvertFrom(context, culture, value!);

        var valueAsSpan = valueAsString.AsSpan();
        var separatorIndex = valueAsSpan.IndexOf(';');
        if (separatorIndex != -1) // Has 2de part
        {
            // In V1 if the 2nd part is not empty and not "ISO-4217" then it is a non-iso4217 currency,
            // but in V2 it doesn't matter because currency codes are unique
            var currencyCode = valueAsSpan.Slice(0, separatorIndex).ToString();
            return (Currency)CurrencyInfo.FromCode(currencyCode);
        }

        return (Currency)CurrencyInfo.FromCode(valueAsSpan.ToString());
    }

    /// <summary>Converts the given value object to the specified type, using the specified context and culture information.</summary>
    /// <returns>An <see cref="object" /> that represents the converted value.</returns>
    /// <param name="context">An <see cref="ITypeDescriptorContext" /> that provides a format context. </param>
    /// <param name="culture">A <see cref="CultureInfo" />. If null is passed, the current culture is assumed. </param>
    /// <param name="value">The <see cref="object" /> to convert. </param>
    /// <param name="destinationType">The <see cref="Type" /> to convert the <paramref name="value" /> parameter to. </param>
    /// <exception cref="ArgumentNullException">The <paramref name="destinationType" /> parameter is null. </exception>
    /// <exception cref="NotSupportedException">The conversion cannot be performed. </exception>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
            if (destinationType == typeof(string) && value is Currency currency)
            {
                    var result = new StringBuilder();
                    result.Append(currency.Code);
                    return result.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

    // IsValid(ITypeDescriptorContext, Object)
}
