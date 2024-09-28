using Microsoft.AspNetCore.Mvc;

namespace NumericParser.Web.Controllers;

[ApiController]
[Route("/")]
public class ParserController : ControllerBase
{
	private const int MaxBatchSize = 10_000;

	private const string InputTooLongError = "Input too long";
	private const string InvalidInputError = "Invalid input";

	private readonly DecimalParserSettings preferThousandsSettings = new() { PreferThousandsInAmbiguousCase = true };

	[HttpGet("parse")]
	public ParseResult ParseSingleValue([FromQuery] string query, [FromQuery] bool preferThousands)
	{
		var settings = preferThousands ? preferThousandsSettings : DecimalParserSettings.Default;
		return ProcessQuery(query, settings);
	}

	[HttpPost("parse")]
	public ActionResult<IReadOnlyCollection<ParseResult>> ParseBatch([FromBody] BatchQuery query)
	{
		if (query.Source == null || query.Source.Count == 0)
			return Array.Empty<ParseResult>();

		if (query.Source.Count > MaxBatchSize)
			return BadRequest($"Batch size too large. Max is {MaxBatchSize}");

		var settings = query.PreferThousands
			? preferThousandsSettings
			: DecimalParserSettings.Default;

		return query.Source
			.Select(q => ProcessQuery(q, settings))
			.ToArray();
	}

	private static ParseResult ProcessQuery(string query, DecimalParserSettings settings)
	{
		if (query.Length > DecimalParser.MaximumInputLength)
			return new ParseResult { Source = query, Error = InputTooLongError };

		return new ParseResult
		{
			Source = query,
			Value = query.TryParseDecimal(out var result, settings) ? result : null,
			Error = result == null ? InvalidInputError : null
		};
	}
}
