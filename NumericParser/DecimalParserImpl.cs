using System.Globalization;
using System.Text.RegularExpressions;

namespace NumericParser;

internal static partial class Regexes
{
#if NETSTANDARD2_1
	public static Regex SpacesPattern() => _spacesPattern.Value;
	public static Regex DecimalPattern() => _decimalPattern.Value;
	public static Regex ExponentPattern() => _exponentPattern.Value;

	private static readonly Lazy<Regex> _spacesPattern = new(() => new Regex(@"\s"));
	private static readonly Lazy<Regex> _decimalPattern = new(() => new Regex(@"[\d\.\,\s]*"));
	private static readonly Lazy<Regex> _exponentPattern = new(() => new Regex(@"[-+]?\d*\.?\d+[eE][-+]?\d+"));
#else
	[GeneratedRegex(@"\s", RegexOptions.IgnoreCase, "en-US")]
	public static partial Regex SpacesPattern();

	[GeneratedRegex(@"[\d\.\,\s]*", RegexOptions.IgnoreCase, "en-US")]
	public static partial Regex DecimalPattern();

	[GeneratedRegex(@"[-+]?\d*\.?\d+[eE][-+]?\d+", RegexOptions.IgnoreCase, "en-US")]
	public static partial Regex ExponentPattern();
#endif
}

internal static class DecimalParserImpl
{
	private static readonly Lazy<NumberFormatInfo> CommaFormatInfo = new(CreateCommaFormat);
	private static readonly Lazy<NumberFormatInfo> DotFormatInfo = new(CreateDotFormat);

	public static decimal? ParseDecimal(this string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		if (Regexes.ExponentPattern().IsMatch(value))
		{
			return value.AsSpan().TryParseExponent();
		}

		Span<char> v = stackalloc char[value.Length];
		var j = 0;
		for (int i = 0; i < value.Length; i++)
		{
			var c = value[i];
			if (!char.IsWhiteSpace(c))
				v[j++] = c;
		}

		v = v[..j];

#if NETSTANDARD2_1
		if (!Regexes.DecimalPattern().IsMatch(v.ToString()))
#else
		if (!Regexes.DecimalPattern().IsMatch(v))
#endif
		{
			return null;
		}

		if (v.Contains(',') && v.Contains('.'))
		{
			var last = v.LastIndexOfAny([',', '.']);
			var c = v[last];
			return v.Count(c) == 1
				? v.TryParse(c == '.' ? Format.Dot : Format.Comma)
				: null;
		}

		if (v.Contains(','))
		{
			return v.Count(',') == 1
				? v.TryParse(Format.Comma)
				: v.TryParse(Format.Dot);
		}

		if (v.Contains('.'))
		{
			return v.Count('.') == 1
				? v.TryParse(Format.Dot)
				: v.TryParse(Format.Comma);
		}

		return v.TryParse(Format.Dot);
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

	private static decimal? TryParseExponent(this ReadOnlySpan<char> value)
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
