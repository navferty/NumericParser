# NumericParser

Simple library for flexible parsing numeric values, stored in strings, to decimal values with precision.

Handles with different formats like "1,111.11" or "2.222,22".

## NuGet package installation

[![NuGet](https://img.shields.io/nuget/v/NumericParser.svg)](https://www.nuget.org/packages/NumericParser/)

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
using NumericParser;

var settings = new DecimalParserSettings { PreferThousandsInAmbiguousCases = true };

var oldvalue = "1,234";
var newValue = oldvalue.ParseDecimal(settings);
Console.WriteLine($"Parsed value: {newValue}"); // Parsed value: 1234m
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
BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4894/22H2/2022Update)
Intel Core i7-10750H CPU 2.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.400
  [Host]   : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  ShortRun : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2

Job=ShortRun  IterationCount=3  LaunchCount=1
WarmupCount=3
```

| Method                                  | Input                | Mean          | Error           | StdDev        | Allocated |
|---------------------------------------- |--------------------- |--------------:|----------------:|--------------:|----------:|
| **TryParseDecimal_DefaultSettings**     | **?**                |  **9.423 ns** |   **0.6511 ns** | **0.0357 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | ?                    |      9.859 ns |       4.8461 ns |     0.2656 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | ****                 |  **9.674 ns** |   **3.5315 ns** | **0.1936 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings |                      |      9.331 ns |       3.5033 ns |     0.1920 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **-1 000 000 . 321** |**114.051 ns** |  **55.3358 ns** | **3.0331 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | -1 000 000 . 321     |    136.274 ns |      56.0379 ns |     3.0716 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **-5.371E8**         | **82.080 ns** |  **47.8141 ns** | **2.6209 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | -5.371E8             |     78.723 ns |      54.0351 ns |     2.9618 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **.123456789**       |**106.333 ns** | **110.8934 ns** | **6.0784 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | .123456789           |    113.011 ns |      60.4266 ns |     3.3122 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **1**                | **70.065 ns** |  **99.4505 ns** | **5.4512 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 1                    |     61.885 ns |       7.1367 ns |     0.3912 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **11111**            | **87.614 ns** | **131.9055 ns** | **7.2302 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 11111                |     83.296 ns |      57.4872 ns |     3.1511 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **12,345,678.91**    |**139.126 ns** |  **56.2725 ns** | **3.0845 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 12,345,678.91        |    137.387 ns |      16.0171 ns |     0.8779 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **12,91**            | **78.542 ns** |   **6.4591 ns** | **0.3540 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 12,91                |     78.726 ns |      11.8584 ns |     0.6500 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **12.345.678,91**    |**136.823 ns** |  **17.5369 ns** | **0.9613 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 12.345.678,91        |    140.893 ns |      95.6219 ns |     5.2414 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **123.456.789**      |**130.866 ns** |  **47.8205 ns** | **2.6212 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 123.456.789          |    134.878 ns |      92.6385 ns |     5.0778 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **123.4(...)67890 [68|**427.403 ns** |  **95.8469 ns** | **5.2537 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 123.4(...)67890 [68] |    454.190 ns |     169.8326 ns |     9.3091 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **1234567.89**       |**103.208 ns** |  **92.1692 ns** | **5.0521 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 1234567.89           |    107.756 ns |     129.0364 ns |     7.0729 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **15E3**             | **61.500 ns** |  **23.5691 ns** | **1.2919 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 15E3                 |     60.650 ns |      24.8720 ns |     1.3633 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **34.56**            | **72.987 ns** |   **1.4223 ns** | **0.0780 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | 34.56                |     73.940 ns |       3.9875 ns |     0.2186 ns |      -    |
| **TryParseDecimal_DefaultSettings**     | **invalid**          | **13.216 ns** |   **2.1263 ns** | **0.1165 ns** |  **-**    |
| TryParseDecimal_PreferThousandsSettings | invalid              |     14.079 ns |       1.2023 ns |     0.0659 ns |      -    |

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
