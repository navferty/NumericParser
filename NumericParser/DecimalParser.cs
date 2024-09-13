using System.Diagnostics.CodeAnalysis;

namespace NumericParser;

public static class DecimalParser
{
	/// <summary>
	/// Try parse input string as decimal.
	/// </summary>
	/// <param name="value">Input string.</param>
	/// <param name="parsed">Decimal value.</param>
	/// <returns>True if parsing was successfull, otherwise false.</returns>
	public static bool TryParseDecimal(this string? value, [NotNullWhen(true)]out decimal? parsed)
	{
		var result = DecimalParserImpl.ParseDecimal(value);
		if (result.HasValue)
		{
			parsed = result.Value;
			return true;
		}

		parsed = null;
		return false;
	}

	/// <summary>
	/// Parse input string as decimal or throw <see cref="ArgumentException"/> if value could not be parsed.
	/// </summary>
	/// <param name="value">Input string.</param>
	/// <returns>Decimal value.</returns>
	/// <exception cref="ArgumentException">Throws if value can not be parsed to decimal.</exception>
	public static decimal ParseDecimal(this string? value)
	{
		return DecimalParserImpl.ParseDecimal(value)
			?? throw new ArgumentException("Failed to parse value to decimal");
	}
}
