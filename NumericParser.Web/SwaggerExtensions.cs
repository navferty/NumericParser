namespace NumericParser.Web;

internal static class SwaggerExtensions
{
	public static IServiceCollection AddSwagger(this IServiceCollection services)
	{
		services
			.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new()
				{
					Title = "NumericParser simple API",
					Version = "v1",
					Description = "Simple API to parse numbers from strings",
					Contact = new()
					{
						Name = "navferty",
						Email = "navferty@ymail.com",
						Url = new("https://navferty.com"),
					},
					License = new()
					{
						Name = "The MIT License (MIT)",
						Url = new("https://mit-license.org/"),
					},
				});

				// include XML comments in Swagger
				var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				options.IncludeXmlComments(xmlPath);
			});

		return services;
	}
}
