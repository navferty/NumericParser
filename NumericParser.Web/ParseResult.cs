namespace NumericParser.Web;

public sealed record ParseResult
{
	public required string Source { get; init; }
	public decimal? Value { get; init; }
	public string? Error { get; init; }
}
