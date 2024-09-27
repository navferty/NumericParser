using BenchmarkDotNet.Attributes;

namespace NumericParser.Benchmarks;

[MemoryDiagnoser, ShortRunJob]
public class NumericParserBenchmarks
{
	private DecimalParserSettings defaultSettings = null!;
	private DecimalParserSettings preferThousandsSettings = null!;

	[Params(
		"1",
		"11111",
		"12,91",
		"34.56",
		"12.345.678,91",
		"12,345,678.91",
		"-1 000 000 . 321",
		"1234567.89",
		".123456789",
		"123.456.789",
		"123.456.789.123.456.789.123.456.789.123.456.789,12345678901234567890",
		"15E3",
		"-5.371E8",
		"invalid",
		"",
		null)]
	public string? Input { get; set; }

	[GlobalSetup]
	public void Setup()
	{
		defaultSettings = new DecimalParserSettings();
		preferThousandsSettings = new DecimalParserSettings { PreferThousandsInAmbiguousCase = true };
	}

	[Benchmark]
	public bool TryParseDecimal_DefaultSettings()
	{
		return Input.TryParseDecimal(out _, defaultSettings);
	}

	[Benchmark]
	public bool TryParseDecimal_PreferThousandsSettings()
	{
		return Input.TryParseDecimal(out _, preferThousandsSettings);
	}
}
