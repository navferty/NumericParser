using NumericParser;
using Xunit;

namespace NumericParserTests;

public class NumericParserTests
{
	[Theory]
	[MemberData(nameof(GetData))]
	public void ParseDecimal(string sourceValue, bool parsed, decimal? targetValue)
	{
		var parsedActual = sourceValue.TryParseDecimal(out var result);

		Assert.Equal(parsed, parsedActual);
		Assert.Equal(targetValue, result);
	}

	public static TheoryData<string, bool, decimal?> GetData()
	{
		return new TheoryData<string, bool, decimal?>
		{
			{ "0", true, 0m },
			{ "123", true, 123m },
			{ "1.1", true, 1.1m },
			{ "1,1", true, 1.1m },
			{ "1 000", true, 1000m },
			{ "12  00 00 . 12", true, 120000.12m },
			{ "0.1", true, 0.1m },
			{ ".123", true, 0.123m },
			{ "10.000.000", true, 10000000m },
			{ "10.000,12", true, 10000.12m },
			{ "12,345.12", true, 12345.12m },
			{ "12,345,000", true, 12345000m },
			{ "1E6", true, 1000000m },
			{ "1E-7", true, .0000001m },
			{ "1.2E3", true, 1200m },
			{ "-1.3E-5", true, -0.000013m },
			{ "no value", false, null },
			{ "00000011111111222222222X", false, null },
			{ new string('a', 10_000), false, null },
			{ new string('1', 10_000), false, null }, // too big value
			{ "冬卉", false, null }, // 4-byte symbols
		};
	}
}
