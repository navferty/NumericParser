using System.Globalization;

namespace NumericParser;

internal static class DecimalParserImpl
{
	private static readonly Lazy<NumberFormatInfo> CommaFormatInfo = new(CreateCommaFormat);
	private static readonly Lazy<NumberFormatInfo> DotFormatInfo = new(CreateDotFormat);

	public static decimal? ParseDecimal(this string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return null;
		}

		Span<char> value = stackalloc char[input.Length];
		var j = 0;
		for (int i = 0; i < input.Length; i++)
		{
			var c = input[i];
			if (!char.IsWhiteSpace(c))
				value[j++] = c;
		}

		value = value[..j];

#if NETSTANDARD2_1
		if (!Regexes.DecimalPattern().IsMatch(value.ToString()))
#else
		if (!Regexes.DecimalPattern().IsMatch(value))
#endif
		{
			return null;
		}

#if NETSTANDARD2_1
		if (Regexes.ExponentPattern().IsMatch(value.ToString()))
#else
		if (Regexes.ExponentPattern().IsMatch(value))
#endif
		{
			return value.TryParseExponent();
		}

		if (value.Contains(',') && value.Contains('.'))
		{
			var last = value.LastIndexOfAny([',', '.']);
			var c = value[last];
			return value.Count(c) == 1
				? value.TryParse(c == '.' ? Format.Dot : Format.Comma)
				: null;
		}

		if (value.Contains(','))
		{
			return value.Count(',') == 1
				? value.TryParse(Format.Comma)
				: value.TryParse(Format.Dot);
		}

		if (value.Contains('.'))
		{
			return value.Count('.') == 1
				? value.TryParse(Format.Dot)
				: value.TryParse(Format.Comma);
		}

		return value.TryParse(Format.Dot);
	}

#if NETSTANDARD2_1
	private static bool Contains(this Span<char> value, char c)
	{
		return value.IndexOf(c) != -1;
	}

	private static int Count(this Span<char> value, char c)
	{
		var counter = 0;
		foreach (var v in value)
		{
			if (v == c)
				counter++;
		}
		return counter;
	}
#endif

	private static decimal? TryParseExponent(this Span<char> value)
	{
		return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result)
			? result
			: null;
	}

	private static decimal? TryParse(this Span<char> value, Format info)
	{
		var formatInfo = info == Format.Comma
			? CommaFormatInfo.Value
			: DotFormatInfo.Value;

		// TODO add test-cases for currencies formats
		//formatInfo.CurrencyNegativePattern = 8;
		//formatInfo.CurrencyPositivePattern = 3;
		return decimal.TryParse(value, NumberStyles.Currency, formatInfo, out decimal result)
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
		Dot,
		Comma
	}
}
