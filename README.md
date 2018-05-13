# NumericParser
Parse numeric values from string to decimal

Simple library to parse numeric values, stored in strings, to decimal values with precision

Handles with different formats like "1,111.11" or "2.222,22"

Usage example:
```csharp
using NumericParser;
namespace Test
{
  class Test
  {
    void ParseDemo()
    {
      var oldvalue = "123,456.78"
      var newValue = oldvalue.ParseDecimal();
      if (newValue == null)
      {
        throw new InvalidOperationException("Invalid input value");
      }
    }
  }
}
```

Test cases:

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
