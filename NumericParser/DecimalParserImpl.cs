using System.Globalization;

namespace NumericParser;

internal static class DecimalParserImpl
{
	private static readonly Lazy<NumberFormatInfo> CommaFormatInfo = new(CreateCommaFormat);
	private static readonly Lazy<NumberFormatInfo> DotFormatInfo = new(CreateDotFormat);

	public static decimal? ParseDecimal(this string? input)
	{
		if (string.IsNullOrEmpty(input) || input!.Length > DecimalParser.MaximumInputLength)
		{
			return null;
		}

		Span<char> value = stackalloc char[input.Length];
		var copyResult = InputReader.CopyWithoutSpaces(input, value);
		if (copyResult.BytesWritten == -1)
		{
			return null;
		}

		value = value.Slice(0, copyResult.BytesWritten);

		if (copyResult.IsExponent)
		{
			return value.TryParseExponent();
		}

		if (copyResult.CommasCount > 0 && copyResult.DotsCount > 0)
		{
			var lastSeparator = copyResult.LastSeparator;

			if (lastSeparator == '.' && copyResult.DotsCount == 1)
			{
				return value.TryParse(Format.Dot);
			}

			if (lastSeparator == ',' && copyResult.CommasCount == 1)
			{
				return value.TryParse(Format.Comma);
			}

			return null;
		}

		if (copyResult.CommasCount > 0)
		{
			return copyResult.CommasCount == 1
				? value.TryParse(Format.Comma)
				: value.TryParse(Format.Dot);
		}

		if (copyResult.DotsCount > 0)
		{
			return copyResult.DotsCount == 1
				? value.TryParse(Format.Dot)
				: value.TryParse(Format.Comma);
		}

		return value.TryParse(Format.Dot);
	}

	private static decimal? TryParseExponent(this Span<char> value)
	{
#if NETSTANDARD2_0
		var valueToBeParsed = value.ToString();
#else
		var valueToBeParsed = value;
#endif
		return decimal.TryParse(valueToBeParsed, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result)
			? result
			: null;
	}

	private static decimal? TryParse(this Span<char> value, Format info)
	{
#if NETSTANDARD2_0
		var valueToBeParsed = value.ToString();
#else
		var valueToBeParsed = value;
#endif
		var formatInfo = info == Format.Comma
			? CommaFormatInfo.Value
			: DotFormatInfo.Value;

		// TODO add test-cases for currencies formats
		//formatInfo.CurrencyNegativePattern = 8;
		//formatInfo.CurrencyPositivePattern = 3;
		return decimal.TryParse(valueToBeParsed, NumberStyles.Currency, formatInfo, out decimal result)
			? result
			: null;
	}

	private static NumberFormatInfo CreateDotFormat()
	{
		NumberFormatInfo dotFormatInfo = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
		dotFormatInfo.CurrencyDecimalSeparator = ".";
		dotFormatInfo.CurrencyGroupSeparator = ",";
		dotFormatInfo.NumberDecimalSeparator = ".";
		dotFormatInfo.NumberGroupSeparator = ",";
		return dotFormatInfo;
	}

	private static NumberFormatInfo CreateCommaFormat()
	{
		NumberFormatInfo commaFormatInfo = (NumberFormatInfo)NumberFormatInfo.InvariantInfo.Clone();
		commaFormatInfo.CurrencyDecimalSeparator = ",";
		commaFormatInfo.CurrencyGroupSeparator = ".";
		commaFormatInfo.NumberDecimalSeparator = ",";
		commaFormatInfo.NumberGroupSeparator = ".";
		return commaFormatInfo;
	}

	private enum Format
	{
		/// <summary>
		/// Decimal separator is dot.
		/// </summary>
		Dot,

		/// <summary>
		/// Decimal separator is comma.
		/// </summary>
		Comma
	}
}
