# NumericParser

Simple library for parsing numeric values, stored in strings, to decimal values with precision.

Handles with different formats like "1,111.11" or "2.222,22".

[![NuGet](https://img.shields.io/nuget/v/NumericParser.svg)](https://www.nuget.org/packages/NumericParser/)

## NuGet package installation

You can download NumericParser from [nuget.org](https://www.nuget.org/packages/NumericParser/) as NuGet package,
or simply run the following command in the Package Manager Console:

```powershell
NuGet\Install-Package NumericParser
```

## Usage example

```csharp
using NumericParser;

var oldvalue = "1,234,567.8";
var newValue = oldvalue.ParseDecimal();
Console.WriteLine($"Parsed value: {newValue}");

var oldvalue1 = "1.234.567,8";
if (oldvalue1.TryParseDecimal(out var parsed))
{
  Console.WriteLine($"Parsed value: {parsed}");
}
```

## Additional settings

There are some additional settings that can be used to customize the parsing process. Currently, the following settings are available:

* `DecimalParserSettings.PreferThousandsInAmbiguousCases` - if set to true, the parser will prefer the thousands separator in ambiguous cases. For example, if the input is "1,234" or "1.234", the parser will treat it as 1234m (and not as 1.234m) if this setting is set to true. By default, this setting is set to false.

```csharp
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
|"1.2E3"		|		1200	|
|"-1.3e-5"		|		0.000013|
|"no value"		|		null	|
|null			|		null	|
| "10.001" | 10.001m \* |
| "100,123" | 100.123m \* |
| "12,345.12" | 12345.12m |
| "12,345,000" | 12345000m |
| "12,34,56,789.12" | 123456789.12m |
| "12.34.56.789,12" | 123456789.12m |
| "12,3456,7890,2345.67" | 12345678902345.67m |
| "12.3456.7890.2345,67" | 12345678902345.67m |

\*Depends on [additional settings](#Additional-settings).

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
```

| Method           | Input                | Mean          | Error         | StdDev       | Allocated     |
|----------------- |---------------------:|--------------:|--------------:|-------------:|--------------:|
| **ParseDecimal** | **-1 000 000 . 321** | **107.32 ns** | **42.899 ns** | **2.351 ns** |         **-** |
| **ParseDecimal** | **-5.371E8**         |  **65.59 ns** |  **6.806 ns** | **0.373 ns** |         **-** |
| **ParseDecimal** | **.123456789**       |  **91.14 ns** |  **5.553 ns** | **0.304 ns** |         **-** |
| **ParseDecimal** | **1**                |  **52.02 ns** | **11.685 ns** | **0.640 ns** |         **-** |
| **ParseDecimal** | **11**               |  **58.21 ns** | **21.371 ns** | **1.171 ns** |         **-** |
| **ParseDecimal** | **111**              |  **63.55 ns** | **17.702 ns** | **0.970 ns** |         **-** |
| **ParseDecimal** | **1111**             |  **64.94 ns** |  **8.368 ns** | **0.459 ns** |         **-** |
| **ParseDecimal** | **11111**            |  **77.82 ns** | **59.420 ns** | **3.257 ns** |         **-** |
| **ParseDecimal** | **12,345,678.91**    | **124.54 ns** | **16.657 ns** | **0.913 ns** |         **-** |
| **ParseDecimal** | **12,91**            |  **66.78 ns** | **21.243 ns** | **1.164 ns** |         **-** |
| **ParseDecimal** | **12.345.678,91**    | **118.17 ns** | **23.138 ns** | **1.268 ns** |         **-** |
| **ParseDecimal** | **123.456.789**      | **124.79 ns** | **16.812 ns** | **0.922 ns** |         **-** |
| **ParseDecimal** | **1234567.89**       |  **92.64 ns** | **80.358 ns** | **4.405 ns** |         **-** |
| **ParseDecimal** | **15E3**             |  **49.84 ns** |  **8.490 ns** | **0.465 ns** |         **-** |
| **ParseDecimal** | **34.56**            |  **66.46 ns** |  **8.327 ns** | **0.456 ns** |         **-** |

```
* Legends *
Input     : Value of the 'Input' parameter
Mean      : Arithmetic mean of all measurements
Error     : Half of 99.9% confidence interval
StdDev    : Standard deviation of all measurements
Allocated : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
1 ns      : 1 Nanosecond (0.000000001 sec)
```
<details>
<summary>Benchmark results for .NET Framework 4.8.1</summary>

```
BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4780/22H2/2022Update)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
  [Host]   : .NET Framework 4.8.1 (4.8.9261.0), X64 RyuJIT VectorSize=256
  ShortRun : .NET Framework 4.8.1 (4.8.9261.0), X64 RyuJIT VectorSize=256

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3  
```


| Method           | Input                | Mean         | Error         | StdDev       | Gen0       | Allocated     |
|----------------- |---------------------:|-------------:|--------------:|-------------:|-----------:|--------------:|
| **ParseDecimal** | **.123456789**       | **260.0 ns** |  **75.70 ns** |  **4.15 ns** |      **-** |         **-** |
| **ParseDecimal** | **1**                | **128.1 ns** |  **77.30 ns** |  **4.24 ns** |      **-** |         **-** |
| **ParseDecimal** | **-1 000 000 . 321** | **309.6 ns** |  **20.58 ns** |  **1.13 ns** | **0.0086** |      **56 B** |
| **ParseDecimal** | **11**               | **143.8 ns** |  **16.19 ns** |  **0.89 ns** |      **-** |         **-** |
| **ParseDecimal** | **111**              | **163.0 ns** |  **51.93 ns** |  **2.85 ns** |      **-** |         **-** |
| **ParseDecimal** | **1111**             | **185.6 ns** |   **9.63 ns** |  **0.53 ns** |      **-** |         **-** |
| **ParseDecimal** | **11111**            | **193.8 ns** |  **26.74 ns** |  **1.47 ns** |      **-** |         **-** |
| **ParseDecimal** | **12,345,678.91**    | **323.8 ns** | **509.67 ns** | **27.94 ns** |      **-** |         **-** |
| **ParseDecimal** | **12,91**            | **178.4 ns** |  **10.18 ns** |  **0.56 ns** |      **-** |         **-** |
| **ParseDecimal** | **12.345.678,91**    | **321.7 ns** | **123.22 ns** |  **6.75 ns** |      **-** |         **-** |
| **ParseDecimal** | **123.456.789**      | **290.0 ns** |   **8.04 ns** |  **0.44 ns** |      **-** |         **-** |
| **ParseDecimal** | **1234567.89**       | **262.5 ns** |  **49.36 ns** |  **2.71 ns** |      **-** |         **-** |
| **ParseDecimal** | **15E3**             | **161.5 ns** |   **9.60 ns** |  **0.53 ns** |      **-** |         **-** |
| **ParseDecimal** | **34.56**            | **179.0 ns** |  **39.57 ns** |  **2.17 ns** |      **-** |         **-** |
| **ParseDecimal** | **-5.371E8**         | **226.3 ns** |  **10.25 ns** |  **0.56 ns** |      **-** |         **-** |

</details>
