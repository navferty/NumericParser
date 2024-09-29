using Microsoft.AspNetCore.Mvc;

namespace NumericParser.Web.Controllers;

[ApiController]
[Route("/")]
public class ParserController : ControllerBase
{
	private const int MaxBatchSize = 10_000;

	private readonly DecimalParserSettings preferThousandsSettings = new()
	{
		PreferThousandsInAmbiguousCase = true,
	};

	/// <summary>
	/// Parse single value from query string.
	/// </summary>
	/// <param name="query">A number to parse.</param>
	/// <param name="preferThousands">Should prefer thousands in ambiguous cases like "123,456" or "321.654".</param>
	/// <returns>Result object with "source", "value" and "error" fields. "source" matches initial value.
	/// Either "value" or "error" is present, depending on success of parsing.</returns>
	[HttpGet("parse")]
	[ProducesResponseType(typeof(ParseResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ParseResult), StatusCodes.Status400BadRequest)]
	public ActionResult<ParseResult> ParseSingleValue([FromQuery] string query, [FromQuery] bool? preferThousands = null)
	{
		var settings = preferThousands ?? false
			? preferThousandsSettings
			: DecimalParserSettings.Default;
		var result = ProcessQuery(query, settings);

		return result.Error == null
			? Ok(result)
			: BadRequest(result);
	}

	/// <summary>
	/// Parse batch of string values into decimal numbers. Maximum batch size is 10 000 (ten thousand).
	/// </summary>
	/// <param name="query">Object with "source" as array of strings and "preferThousands" as boolean fields.</param>
	/// <returns>
	/// List of parsed result in same order as input.
	/// Each object contains "source", "value" and "error" fields.
	/// "source" matches initial value.
	/// Either "value" or "error" is present, depending on success of parsing.
	/// </returns>
	[HttpPost("parse")]
	[ProducesResponseType(typeof(ParseResult), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
	public ActionResult<IReadOnlyCollection<ParseResult>> ParseBatch([FromBody] BatchQuery query)
	{
		if (query.Source == null || query.Source.Count == 0)
			return Array.Empty<ParseResult>();

		if (query.Source.Count > MaxBatchSize)
		{
			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Batch size too large",
				Detail = $"The maximum batch size is {MaxBatchSize}.",
				Instance = HttpContext.Request.Path
			};
			return BadRequest(problemDetails);
		}

		var settings = query.PreferThousands ?? false
			? preferThousandsSettings
			: DecimalParserSettings.Default;

		return query.Source
			.Select(q => ProcessQuery(q, settings))
			.ToArray();
	}

	private static ParseResult ProcessQuery(string query, DecimalParserSettings settings)
	{
		const string InputTooLongError = "Input too long";
		const string InvalidInputError = "Invalid input";

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
