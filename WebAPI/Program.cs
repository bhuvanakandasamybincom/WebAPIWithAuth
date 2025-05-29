using BoardCasterWebAPI.Data;
using BoardCasterWebAPI.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using WebAPI.Middleware;
var builder = WebApplication.CreateBuilder(args);


//Register IHttpClientFactory (best practice for managing HttpClient instances)
builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//To limit the number of API requests per second by using SlidingPolicy
builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("SlidingPolicy", policy =>
    {
        policy.PermitLimit = 10;
        policy.Window = TimeSpan.FromSeconds(10);
        policy.SegmentsPerWindow = 10;
        policy.QueueLimit = 10;
        policy.QueueProcessingOrder = QueueProcessingOrder.NewestFirst;
    });
});

builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
//});

//Add cors services
builder.Services.AddCors(
    options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
    });

//Register in Dependency Injection (DI)
//DI patter that ensures loosly coupled
builder.Services.AddSingleton<IMessageWriter, MessageWriter>();
//builder.Services.AddTransient<ExceptionHandleMiddleware>();

GlobalModel.SecKey = builder.Configuration.GetSection("JWT").GetSection("secret_key").Value;
GlobalModel.SecIssuer = builder.Configuration.GetSection("JWT").GetSection("issuer").Value;
GlobalModel.SecAudience = builder.Configuration.GetSection("JWT").GetSection("audience").Value;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //options.Authority = builder.Configuration["JWT:Authority"];
    //options.Audience = builder.Configuration["Api:Audience"];
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = GlobalModel.SecIssuer,
        ValidAudience = GlobalModel.SecAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalModel.SecKey)),
        ClockSkew = TimeSpan.Zero
    };
});

//Policy based role checks
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
         policy => policy.RequireRole("Administrator"));
    options.AddPolicy("ElevatedRights", policy =>
          policy.RequireRole("Administrator", "PowerUser", "BackupAdministrator"));
});


var app = builder.Build();

//**********Need to check****************//
//Use in Pipeline
app.UseMiddleware<ExceptionHandleMiddleware>();

//Custom error log middleware to track error
app.UseExceptionHandleMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
//app.UseSwaggerUI(
//    c=>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo web API");
//        c.RoutePrefix = string.Empty;
//    });
}

app.UseHttpsRedirection();

//Routing
app.UseRouting();
//Auth
app.UseAuthorization();
app.UseAuthentication();


  



//app.UseEndpoints(endpoints => endpoints.MapGet("/", 
//    async context => await context.Response.WriteAsync("Hello World"))
//);

//Controller base
app.MapControllers();

app.Run();
