namespace NumericParser;

public sealed class DecimalParserSettings
{
	public static DecimalParserSettings Default { get; } = new();

	/// <summary>
	/// Prefer thousands in ambiguous cases, when it cannot be determined, for example
	/// <code>123,456</code>. Set to true if such input should be treated as "123 thousands and 456".
	/// </summary>
	public bool PreferThousandsInAmbiguousCase { get; set; }
}
