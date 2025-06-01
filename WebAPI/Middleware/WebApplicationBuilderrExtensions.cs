using Microsoft.OpenApi.Models;

namespace BoardCasterWebAPI.Middleware
{
    public static class WebApplicationBuilderrExtensions
    {
        public static void AddPresentation(this WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddControllers();
            //Add Bearer auth input box in Swagger UI
            builder.Services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("bearerAuth", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme,Id="bearerAuth"}
                        },
                        []
                    }
                });
                        });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
        }
    }
}
