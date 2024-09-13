using BenchmarkDotNet.Running;
using NumericParser.Benchmarks;

var summary = BenchmarkRunner.Run<NumericParserBenchmarks>();
