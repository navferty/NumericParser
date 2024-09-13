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
