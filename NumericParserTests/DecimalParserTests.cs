using NumericParser;
using Xunit;

namespace NumericParserTests;

public class NumericParserTests
{
	[Fact]
	public void ParseSimpleDecimal()
	{
		var sourceValue = "1.234.567,89";

		var success = sourceValue.TryParseDecimal(out var result);

		Assert.True(success);
		Assert.Equal(1234567.89m, result);
	}

	[Theory]
	[MemberData(nameof(GetValidData))]
	public void ParseValidDecimal(string sourceValue, decimal targetValue)
	{
		var success = sourceValue.TryParseDecimal(out var result);

		Assert.True(success);
		Assert.Equal(targetValue, result);
	}

	[Theory]
	[MemberData(nameof(GetInvalidData))]
	public void ParseInvalidDecimal(string? sourceValue)
	{
		var success = sourceValue.TryParseDecimal(out var result);

		Assert.False(success);
	}

	public static TheoryData<string, decimal> GetValidData()
	{
		return new TheoryData<string, decimal>
		{
			{ "0", 0m },
			{ "123", 123m },
			{ "1.1", 1.1m },
			{ "1,1", 1.1m },
			{ "1 000", 1000m },
			{ "12  00 00 . 12", 120000.12m },
			{ "0.1", 0.1m },
			{ ".123", 0.123m },
			{ "10.000.000", 10000000m },
			{ "10.000,12", 10000.12m },
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
