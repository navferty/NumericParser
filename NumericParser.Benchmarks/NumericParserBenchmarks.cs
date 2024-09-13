using BenchmarkDotNet.Attributes;

namespace NumericParser.Benchmarks;

[MemoryDiagnoser, ShortRunJob]
public class NumericParserBenchmarks
{
	private readonly string[] data;

	public NumericParserBenchmarks()
	{
		var rand = new Random(15).Next(0, 1000).ToString();
		data =
		[
			rand,
			"000" + rand,
			"123" + rand,
			"000." + rand,
			"123," + rand,
			"1.000." + rand,
			"1,123," + rand,
			"1,000." + rand,
			"1.123," + rand,
			"12.34.56," + rand,
			"12,34,56." + rand,
			rand + "E5",

		];
	}

	[Benchmark]
	public decimal ParseDecimal()
	{
		var result = 0m;
		foreach (var item in data)
		{
			result += item.ParseDecimal();
		}
		return result;
	}

	[Benchmark]
	public decimal TryParseDecimal()
	{
		var result = 0m;
		foreach (var item in data)
		{
			if (item.TryParseDecimal(out var parsed))
				result += parsed.Value;
		}
		return result;
	}
}
