using System.Globalization;
using System.Text.RegularExpressions;

namespace NumericParser;

internal static partial class Regexes
{
#if NET
	[GeneratedRegex(@"\s", RegexOptions.IgnoreCase, "en-US")]
	public static partial Regex SpacesPattern();

	[GeneratedRegex(@"[\d\.\,\s]*", RegexOptions.IgnoreCase, "en-US")]
	public static partial Regex DecimalPattern();

	[GeneratedRegex(@"[-+]?\d*\.?\d+[eE][-+]?\d+", RegexOptions.IgnoreCase, "en-US")]
	public static partial Regex ExponentPattern();
#else
	public static Regex SpacesPattern() => _spacesPattern.Value;
	public static Regex DecimalPattern() => _decimalPattern.Value;
	public static Regex ExponentPattern() => _exponentPattern.Value;

	private static readonly Lazy<Regex> _spacesPattern = new(() => new Regex(@"\s"));
	private static readonly Lazy<Regex> _decimalPattern = new(() => new Regex(@"[\d\.\,\s]*"));
	private static readonly Lazy<Regex> _exponentPattern = new(() => new Regex(@"[-+]?\d*\.?\d+[eE][-+]?\d+"));
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

		var v = Regexes.SpacesPattern().Replace(value, match => string.Empty);

		if (Regexes.ExponentPattern().IsMatch(v))
		{
			return v.TryParseExponent();
		}

		if (!Regexes.DecimalPattern().IsMatch(value))
		{
			return null;
		}

		if (v.Contains(',') && v.Contains('.'))
		{
			var last = v.LastIndexOfAny([',', '.']);
			var c = v[last];
			return v.CountChars(c) == 1
				? v.TryParse(c == '.' ? Format.Dot : Format.Comma)
				: null;
		}

		if (v.Contains(','))
		{
			return v.CountChars(',') == 1
				? v.TryParse(Format.Comma)
				: v.TryParse(Format.Dot);
		}

		if (v.Contains('.'))
		{
			return v.CountChars('.') == 1
				? v.TryParse(Format.Dot)
				: v.TryParse(Format.Comma);
		}

		return v.TryParse(Format.Dot);
	}

	private static int CountChars(this string value, char c)
	{
		return value.Count(x => x == c);
	}

	private static decimal? TryParseExponent(this string value)
	{
		return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal result)
			? result
			: null;
	}

	private static decimal? TryParse(this string value, Format info)
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
