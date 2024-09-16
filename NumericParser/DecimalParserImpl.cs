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
		var copyResult = InputReader.ReadAsSpan(input, value);
		if (copyResult.BytesWritten == -1)
		{
			return null;
		}

		value = value.Slice(0, copyResult.BytesWritten);

		if (copyResult.IsExponent)
		{
			return copyResult.DeterminedSeparator switch
			{
				'.' => value.TryParseExponent(input, Format.Dot),
				',' => value.TryParseExponent(input, Format.Comma),
				'\0' => value.TryParseExponent(input),
				_ => throw new InvalidOperationException("Unexpected separator character"),
			};
		}

		if (copyResult.DeterminedSeparator == '.')
		{
			return value.TryParse(input, Format.Dot);
		}

		if (copyResult.DeterminedSeparator == ',')
		{
			return value.TryParse(input, Format.Comma);
		}

		if (copyResult.CommasCount > 0 && copyResult.DotsCount > 0)
		{
			var lastSeparator = copyResult.LastSeparator;

			if (lastSeparator == '.' && copyResult.DotsCount == 1)
			{
				return value.TryParse(input, Format.Dot);
			}

			if (lastSeparator == ',' && copyResult.CommasCount == 1)
			{
				return value.TryParse(input, Format.Comma);
			}

			return null;
		}

		if (copyResult.CommasCount > 0)
		{
			return copyResult.CommasCount == 1
				? value.TryParse(input, Format.Comma)
				: value.TryParse(input, Format.Dot);
		}

		if (copyResult.DotsCount > 0)
		{
			return copyResult.DotsCount == 1
				? value.TryParse(input, Format.Dot)
				: value.TryParse(input, Format.Comma);
		}

		return value.TryParse(input, Format.Dot);
	}

	private static decimal? TryParseExponent(this Span<char> value, string originalValue, Format? format = null)
	{
#if NETSTANDARD2_0
		// Avoid new string allocation if possible
		var valueToBeParsed = value.Length == originalValue.Length
			? originalValue
			: value.ToString();
#else
		var valueToBeParsed = value;
#endif
		var formatInfo = format.HasValue
			? format.Value == Format.Comma
				? CommaFormatInfo.Value
				: DotFormatInfo.Value
			: CultureInfo.InvariantCulture.NumberFormat;

		return decimal.TryParse(valueToBeParsed, NumberStyles.Float, formatInfo, out decimal result)
			? result
			: null;
	}

	private static decimal? TryParse(this Span<char> value, string originalValue, Format info)
	{
#if NETSTANDARD2_0
		var valueToBeParsed = value.Length == originalValue.Length
			? originalValue
			: value.ToString();
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
