namespace NumericParser.Web;

public sealed record BatchQuery
{
	public required IReadOnlyCollection<string> Source { get; init; }
	public required bool PreferThousands { get; init; }
}
