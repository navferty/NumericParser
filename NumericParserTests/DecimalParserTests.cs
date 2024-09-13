using NumericParser;
using Xunit;

namespace NumericParserTests;

public class NumericParserTests
{
	[Theory]
	[MemberData(nameof(GetData))]
	public void ParseDecimal(string sourceValue, decimal? targetValue)
	{
		Assert.Equal(targetValue, sourceValue.ParseDecimal());
	}

	public static TheoryData<string, decimal?> GetData()
	{
		return new TheoryData<string, decimal?>
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
			{ "1E6", 1000000m },
			{ "1.2E3", 1200m },
			{ "-1.3E-5", -0.000013m },
			{ "no value", null },
		};
	}
}
