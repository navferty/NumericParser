using BenchmarkDotNet.Running;

namespace NumericParser.Benchmarks.NetFramework481
{
	internal class Program
	{
		static void Main() => BenchmarkRunner.Run<NumericParserBenchmarks>();
	}
}
