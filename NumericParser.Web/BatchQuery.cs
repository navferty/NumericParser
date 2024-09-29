namespace NumericParser.Web;

public sealed record BatchQuery
{
	public required IReadOnlyCollection<string> Source { get; init; }
	public bool? PreferThousands { get; init; }
}
