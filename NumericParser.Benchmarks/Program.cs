using BenchmarkDotNet.Running;
using NumericParser.Benchmarks;

_ = BenchmarkRunner.Run<NumericParserBenchmarks>();
