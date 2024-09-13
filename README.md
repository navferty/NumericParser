# NumericParser
Parse numeric values from string to decimal

Simple library to parse numeric values, stored in strings, to decimal values with precision

Handles with different formats like "1,111.11" or "2.222,22"

## NuGet package

You can download NumericParser from [nuget.org](https://www.nuget.org/packages/NumericParser/) as NuGet package.

## Usage example

```csharp
using NumericParser;

namespace Test;

class Test
{
  void ParseDemo()
  {
    var oldvalue = "1,234,567.8";
    var newValue = oldvalue.ParseDecimal();
    Console.WriteLine($"Parsed value: {newValue}");

    var oldvalue1 = "1.234.567,8";
    if (oldvalue1.TryParseDecimal(out var parsed))
    {
      Console.WriteLine($"Parsed value: {parsed}");
    }
  }
}
```

## Examples

| Input value   | Parsed value  |
| ------------- |:-------------:|
|"0"			|		0		|
|"123"			|		123		|
|"1.1"			|		1.1		|
|"1,1"			|		1.1		|
|"1 000"		|		1000	|
|"12 00 00 . 12"|		120000.12|
|"0.1"			|		0.1		|
|".123"			|		0.123	|
|"10.000.000"	|		10000000|
|"10.000,12"	|		10000.12|
|"12,345.12"	|		12345.12|
|"12,345,000"	|		12345000|
|"1E6"			|		1000000	|
|"1.2E3"		|		1200	|
|"-1.3E-5"		|		0.000013|
|"no value"		|		null	|

## Benchmarks

NumericParser's code is allocation-free, based on `Span<char>` under the hood.

```
BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4780/22H2/2022Update)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.400
  [Host]   : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3

| Method          | Mean     | Error     | StdDev    | Allocated |
|---------------- |---------:|----------:|----------:|----------:|
| ParseDecimal    | 3.593 us | 0.6370 us | 0.0349 us |         - |
| TryParseDecimal | 3.512 us | 0.2880 us | 0.0158 us |         - |

* Legends *
Mean      : Arithmetic mean of all measurements
Error     : Half of 99.9% confidence interval
StdDev    : Standard deviation of all measurements
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 us      : 1 Microsecond (0.000001 sec)
```
