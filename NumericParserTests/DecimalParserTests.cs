using NumericParser;
using NUnit.Framework;

namespace NumericParserTests
{
	[TestFixture(Category = "Unit")]
	public class NumericParserTests
	{
		[TestCase("0",               0)]
		[TestCase("123",             123)]
		[TestCase("1.1",             1.1)]
		[TestCase("1,1",             1.1)]
		[TestCase("1 000",           1000)]
		[TestCase("12  00 00 . 12",  120000.12)]
		[TestCase("0.1",             0.1)]
		[TestCase(".123",            0.123)]
		[TestCase("10.000.000",      10000000)]
		[TestCase("10.000,12",       10000.12)]
		[TestCase("12,345.12",       12345.12)]
		[TestCase("12,345,000",      12345000)]
		[TestCase("1E6",             1000000)]
		[TestCase("1.2E3",           1200)]
		[TestCase("-1.3E-5",         -0.000013)]
		[TestCase("no value",        null)]
		public void ParseDecimal(string sourceValue, decimal? targetValue)
		{
			Assert.AreEqual(targetValue, sourceValue.ParseDecimal());
		}
	}
}
