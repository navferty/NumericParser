using BenchmarkDotNet.Attributes;

namespace NumericParser.Benchmarks.NetFramework481
{
	[MemoryDiagnoser, ShortRunJob]
	public class NumericParserBenchmarks
	{
		[Params(
			"1",
			"11",
			"111",
			"1111",
			"11111",
			"12,91",
			"34.56",
			"12.345.678,91",
			"12,345,678.91",
			"-1 000 000 . 321",
			"1234567.89",
			".123456789",
			"123.456.789",
			"15E3",
			"-5.371E8")]
		public string Input { get; set; }

		[Benchmark]
		public decimal ParseDecimal()
		{
			return Input.ParseDecimal();
		}
	}
}
