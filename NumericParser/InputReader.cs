namespace NumericParser;

internal static class InputReader
{
	public static CopyResult CopyWithoutSpaces(string input, Span<char> value)
	{
		var bytesWritten = 0;
		var dots = 0;
		var commas = 0;
		bool isExponent = false;
		char lastSeparator = '\0';

		for (int i = 0; i < input.Length; i++)
		{
			var c = input[i];

			if (char.IsWhiteSpace(c))
			{
				continue;
			}
			else if (char.IsDigit(c) || c == '+' || c == '-')
			{
				value[bytesWritten++] = c;
			}
			else if (c == '.')
			{
				value[bytesWritten++] = c;
				lastSeparator = c;
				dots++;
			}
			else if (c == ',')
			{
				value[bytesWritten++] = c;
				lastSeparator = c;
				commas++;
			}
			else if (c == 'e' || c == 'E')
			{
				value[bytesWritten++] = c;
				isExponent = true;
			}
			else
			{
				return CopyResult.Empty;
			}
		}

		return new(bytesWritten, isExponent, dots, commas, lastSeparator);
	}
}

internal readonly record struct CopyResult
{
	public static CopyResult Empty = new(-1, false, 0, 0, '\0');

	public CopyResult(int bytesWritten, bool isExponent, int dots, int commas, char lastSeparator)
	{
		BytesWritten = bytesWritten;
		IsExponent = isExponent;
		DotsCount = dots;
		CommasCount = commas;
		LastSeparator = lastSeparator;
	}

	public int BytesWritten { get; }
	public bool IsExponent { get; }
	public int DotsCount { get; }
	public int CommasCount { get; }
	public char LastSeparator { get; }
}