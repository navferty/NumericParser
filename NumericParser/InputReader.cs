namespace NumericParser;

internal static class InputReader
{
	public static CopyResult ReadAsSpan(string source, Span<char> destination)
	{
		var state = new ReaderState();

		for (int i = 0; i < source.Length; i++)
		{
			var currentChar = source[i];

			if (char.IsWhiteSpace(currentChar))
			{
				continue;
			}
			else if (char.IsDigit(currentChar))
			{
				state.CurrentDigitGroupCount++;
				destination[state.BytesWritten++] = currentChar;
			}
			else if (currentChar == '+' || currentChar == '-')
			{
				var processed = ProcessSign(ref destination, ref state, currentChar);
				if (!processed)
					return CopyResult.Empty;
			}
			else if (currentChar == '.')
			{
				var processed = ProcessDot(ref destination, ref state);
				if (!processed)
					return CopyResult.Empty;
			}
			else if (currentChar == ',')
			{
				var processed = ProcessComma(ref destination, ref state);
				if (!processed)
					return CopyResult.Empty;
			}
			else if (currentChar == 'e' || currentChar == 'E')
			{
				var processed = ProcessExponent(ref destination, ref state);
				if (!processed)
					return CopyResult.Empty;
			}
			else
			{
				return CopyResult.Empty;
			}
		}

		return new(
			bytesWritten: state.BytesWritten,
			isExponent: state.ExponentIndex != 0,
			dots: state.DotsCount,
			commas: state.CommasCount,
			lastSeparator: state.LastSeparator,
			determinedSeparator: state.DeterminedDecimalSeparator);

	}

	private static bool ProcessSign(ref Span<char> destination, ref ReaderState state, char c)
	{
		// current position must be either 0, or it is exponent right after e/E
		// NOTE: might be whitespace, like: " 123 E -2" => 1.23m
		if (state.BytesWritten != 0
			&& state.ExponentIndex != state.BytesWritten - 1)
		{
			return false;
		}

		destination[state.BytesWritten++] = c;
		state.CurrentDigitGroupCount = 0;
		return true;
	}

	private static bool ProcessDot(ref Span<char> destination, ref ReaderState state)
	{
		if (state.DotsCount != 0 && state.CommasCount > 1)
			return false;

		// two consecutive separators
		if (state.LastSeparator != '\0'
			&& state.LastSeparatorIndex == state.BytesWritten - 1
			&& state.BytesWritten > 0)
			return false;

		// start of value - should be decimal, but not group separator
		if (state.BytesWritten == 0)
			_ = EnsureDecimalSeparator(ref state, '.');

		// there are at least 2 dots, so that it should be group sep, but not decimal sep
		if (state.DotsCount == 1)
			if (!EnsureDecimalSeparator(ref state, ','))
				return false;
		if (state.DotsCount == 0 && state.CommasCount > 1)
			if (!EnsureDecimalSeparator(ref state, '.'))
				return false;
		if (state.DotsCount > 0 && state.CommasCount == 1)
			if (!EnsureDecimalSeparator(ref state, ','))
				return false;

		// avoid short groups of digits if we know that dot is group separator
		if (state.DeterminedDecimalSeparator == ','
			&& state.CurrentDigitGroupCount < 2)
			return false;

		destination[state.BytesWritten] = '.';
		state.LastSeparator = '.';
		state.LastSeparatorIndex = state.BytesWritten;
		state.DotsCount++;
		state.CurrentDigitGroupCount = 0;
		state.BytesWritten++;
		return true;
	}

	private static bool ProcessComma(ref Span<char> destination, ref ReaderState state)
	{
		if (state.CommasCount != 0 && state.DotsCount > 1)
			return false;

		if (state.LastSeparator != '\0'
			&& state.LastSeparatorIndex == state.BytesWritten - 1
			&& state.BytesWritten > 0)
			return false;

		if (state.BytesWritten == 0)
			_ = EnsureDecimalSeparator(ref state, ',');

		// there are at least 2 commas, so that it should be group sep, but not decimal sep
		if (state.CommasCount == 1)
			if (!EnsureDecimalSeparator(ref state, '.'))
				return false;

		if (state.CommasCount == 0 && state.DotsCount > 1)
			if (!EnsureDecimalSeparator(ref state, ','))
				return false;

		if (state.CommasCount > 0 && state.DotsCount == 1)
			if (!EnsureDecimalSeparator(ref state, '.'))
				return false;

		// avoid short groups of digits if we know that comma is group separator
		if (state.DeterminedDecimalSeparator == '.'
			&& state.CurrentDigitGroupCount < 2)
			return false;

		destination[state.BytesWritten] = ',';
		state.LastSeparator = ',';
		state.LastSeparatorIndex = state.BytesWritten;
		state.CommasCount++;
		state.CurrentDigitGroupCount = 0;
		state.BytesWritten++;
		return true;
	}

	private static bool ProcessExponent(ref Span<char> destination, ref ReaderState state)
	{
		// more than one e/E
		if (state.ExponentIndex != 0)
			return false;

		destination[state.BytesWritten] = 'E';
		state.ExponentIndex = state.BytesWritten;
		state.BytesWritten++;
		state.CurrentDigitGroupCount = 0;
		return true;
	}

	static bool EnsureDecimalSeparator(ref ReaderState state, char separatorCandidate)
	{
		if (state.DeterminedDecimalSeparator == '\0')
			state.DeterminedDecimalSeparator = separatorCandidate;

		return state.DeterminedDecimalSeparator == separatorCandidate;
	}

	private struct ReaderState
	{
		public int BytesWritten { get; set; }
		public int ExponentIndex { get; set; }
		public int DotsCount { get; set; }
		public int CommasCount { get; set; }
		public char LastSeparator { get; set; }
		public int LastSeparatorIndex { get; set; }
		public int CurrentDigitGroupCount {  get; set; }
		public char DeterminedDecimalSeparator { get; set; }
	}
}

internal readonly record struct CopyResult
{
	public static CopyResult Empty = new(-1, false, 0, 0, '\0', '\0');

	public CopyResult(
		int bytesWritten,
		bool isExponent,
		int dots,
		int commas,
		char lastSeparator,
		char determinedSeparator)
	{
		BytesWritten = bytesWritten;
		IsExponent = isExponent;
		DotsCount = dots;
		CommasCount = commas;
		LastSeparator = lastSeparator;
		DeterminedSeparator = determinedSeparator;
	}

	public int BytesWritten { get; }
	public bool IsExponent { get; }
	public int DotsCount { get; }
	public int CommasCount { get; }
	public char LastSeparator { get; }
	public char DeterminedSeparator { get; }
}