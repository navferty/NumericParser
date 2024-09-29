using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NumericParser.Web;

internal static class JsonOptionsExtensions
{
	public static JsonOptions ConfigureJsonOptions(this JsonOptions options)
	{
		options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
		options.JsonSerializerOptions.AllowTrailingCommas = true;
		options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
		options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
		options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
		return options;
	}
}
