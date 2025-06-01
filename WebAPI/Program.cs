using BoardCasterWebAPI;
using BoardCasterWebAPI.Data;
using BoardCasterWebAPI.DbContexts;
using BoardCasterWebAPI.Interfaces;
using BoardCasterWebAPI.Middleware;
using BoardCasterWebAPI.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using WebAPI.Entities;
using WebAPI.Middleware;
var builder = WebApplication.CreateBuilder(args);


//Register IHttpClientFactory (best practice for managing HttpClient instances)
builder.Services.AddHttpClient();

builder.AddPresentation();

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

// Add services to the container.
builder.Services.AddTransient<IAuthService, AuthService>();
//// Register AuthService in 3 different ways 1. AddTransient, 2.AddScoped, 3.AddSingleton
//2. builder.Services.AddScoped<AuthService>();
//3. builder.Services.AddSingleton<IAuthService, AuthService>();

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

// Adding EF Core with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServerDBConnection"))
    .EnableSensitiveDataLogging());

//// Add Identity
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//{
//    //options.User.AllowedUserNameCharacters = "";
//    options.User.AllowedUserNameCharacters =
//        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
//    //options.User.RequireUniqueEmail = true;
//})
//        .AddEntityFrameworkStores<ApplicationDbContext>()
//        .AddDefaultTokenProviders();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
}).AddEntityFrameworkStores<ApplicationDbContext>();
// Add Identity
//builder.Services.AddIdentityApiEndpoints<ApplicationUser>(options =>
//{
//}).AddEntityFrameworkStores<ApplicationDbContext>();


// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var SecretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// Adding Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    //options.Authority = builder.Configuration["JWT:Authority"];
    //options.Audience = builder.Configuration["Api:Audience"];
    ////builder.Configuration["Jwt:Issuer"],
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
        //ClockSkew = TimeSpan.Zero
    };
});
//Policy based role checks
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy",
         policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy =>
      policy.RequireRole("User"));
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
app.UseAuthentication();
app.UseAuthorization();



//var roleManager = app.Services.GetRequiredService<RoleManager<IdentityRole>>();
//await RoleSeeder.SeedRolesAsync(roleManager);



//app.UseEndpoints(endpoints => endpoints.MapGet("/", 
//    async context => await context.Response.WriteAsync("Hello World"))
//);

//MapIdentityApi along with "api/identity" as a prefix in API endpoints
//app.MapGroup("api/identity").MapIdentityApi<User>();
//app.MapIdentityApi<ApplicationUser>();

//Controller base
app.MapControllers();

app.Run();
