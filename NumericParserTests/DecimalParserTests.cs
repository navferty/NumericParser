using NumericParser;
using Xunit;

namespace NumericParserTests;

public class DecimalParserTests
{
	[Fact]
	public void ParseSimpleDecimal_ShouldReturnTrueAndCorrectValue()
	{
		var sourceValue = "1.234.567,89";

		var success = sourceValue.TryParseDecimal(out var result);

		Assert.True(success);
		Assert.Equal(1234567.89m, result);
	}

	[Theory]
	[MemberData(nameof(GetValidData))]
	public void ParseValidDecimal_ShouldReturnTrueAndCorrectValue(string sourceValue, decimal targetValue)
	{
		var success = sourceValue.TryParseDecimal(out var result);

		Assert.True(success);
		Assert.Equal(targetValue, result);
	}

	[Theory]
	[MemberData(nameof(GetValidDataWithThousands))]
	public void ParseValidDecimalWithPreferThousands_ShouldReturnTrueAndCorrectValue(string sourceValue, decimal targetValue)
	{
		var options = new DecimalParserSettings { PreferThousandsInAmbiguousCase = true };

		var success = sourceValue.TryParseDecimal(out var result, options);

		Assert.True(success);
		Assert.Equal(targetValue, result);
	}

	[Theory]
	[MemberData(nameof(GetInvalidData))]
	public void ParseInvalidDecimal_ShouldReturnFalse(string? sourceValue)
	{
		var success = sourceValue.TryParseDecimal(out var result);

		Assert.False(success);
		Assert.Null(result);
	}

	[Fact]
	public void TryParseDecimal_MaximumLengthExceeded_ShouldReturnFalse()
	{
		var sourceValue = new string('1', DecimalParser.MaximumInputLength + 1);

		var success = sourceValue.TryParseDecimal(out var result);

		Assert.False(success);
		Assert.Null(result);
	}

	[Fact]
	public void ParseDecimal_InputExceedsMaximumLength_ThrowsArgumentException()
	{
		var input = new string('1', DecimalParser.MaximumInputLength + 1);

		Assert.Throws<ArgumentException>(() => DecimalParser.ParseDecimal(input));
	}

	[Fact]
	public void ParseDecimal_MinimumValue_ShouldReturnTrueAndCorrectValue()
	{
		var sourceValue = decimal.MinValue.ToString();

		var success = sourceValue.TryParseDecimal(out var result);

		Assert.True(success);
		Assert.Equal(decimal.MinValue, result);
	}

	[Fact]
	public void ParseDecimal_MaximumValue_ShouldReturnTrueAndCorrectValue()
	{
		var sourceValue = decimal.MaxValue.ToString();

		var success = sourceValue.TryParseDecimal(out var result);

		Assert.True(success);
		Assert.Equal(decimal.MaxValue, result);
	}

	public static TheoryData<string, decimal> GetValidDataWithThousands() =>
		new()
		{
			{ "1,000", 1000m },
			{ "1,234", 1234m },
			{ "1.000", 1000m },
			{ "1.234", 1234m },

			{ "1.234,56", 1234.56m },
			{ "1,234.56", 1234.56m },
			{ "123,456", 123456m },
			{ "123.456", 123456m },

			{ "321,000", 321000m },
			{ "123,234", 123234m },
			{ "1123.000", 1123000m },
			{ "-2321.234", -2321234m },

			{ "1,111,000", 1111000m },
			{ "-2,221,234", -2221234m },

			{ "1.0001", 1.0001m },
			{ "1.23", 1.23m },
		};

	public static TheoryData<string, decimal> GetValidData()
	{
		return new TheoryData<string, decimal>
		{
			{ "0", 0m },
			{ "123", 123m },
			{ "1.1", 1.1m },
			{ "1,1", 1.1m },
			{ "1 000", 1000m },
			{ "123.45", 123.45m },
			{ "123,45", 123.45m },
			{ "1,234.56", 1234.56m },
			{ "1.234,56", 1234.56m },
			{ "123456", 123456m },
			{ "123,456.78", 123456.78m },
			{ "123.456,78", 123456.78m },
			{ "12  00 00 . 12", 120000.12m },
			{ "0.1", 0.1m },
			{ ".123", 0.123m },
			{ "10.000.000", 10000000m },
			{ "10.000,12", 10000.12m },
			{ "10.001", 10.001m },
			{ "100,123", 100.123m },
			{ "12,345.12", 12345.12m },
			{ "12,345,000", 12345000m },
			{ "12,34,56,789.12", 123456789.12m }, // indian system (group by 2 digits)
			{ "12.34.56.789,12", 123456789.12m }, // indian system
			{ "12,3456,7890,2345.67", 12345678902345.67m }, // chinese system (group by 4 digits)
			{ "12.3456.7890.2345,67", 12345678902345.67m }, // chinese system
			{ "123.456", 123.456m },
			{ "123,456", 123.456m },
			{ "1E6", 1000000m },
			{ "1E-7", .0000001m },
			{ "1.2E3", 1200m },
			{ "-1.3E-5", -0.000013m },
			{ "1e6", 1000000m },
			{ "1e-7", .0000001m },
			{ "1.2e3", 1200m },
			{ "-1.3e-5", -0.000013m },
		};
	}

	public static TheoryData<string?> GetInvalidData()
	{
		return new TheoryData<string?>
		{
			{ null },
			{ "" },
			{ "no value" },
			{ "1,234.56.78" },
			{ "1.234,56.78" },
			{ "00000011111111222222222X" },
			{ "1E2E3" },
			{ "1.2eE3" },
			{ "1+2-3" },
			{ "++2" },
			{ "2++" },
			{ "..123" },
			{ "123.." },
			{ ",,123" },
			{ "123,," },
			{ ".,123" },
			{ "123,." },
			{ "123,.456" },
			{ "123.456,789.123,456" }, // mess with separators
			{ "1.1.1.1" },
			{ new string('a', 10_000) },
			{ new string('1', 10_000) }, // too big value
			{ "冬卉" }, // 4-byte symbols
		};
	}
}
