using FlightProject.Models;
using FlightProject.Services;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;

var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow all origins and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173");
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
        });
});

// Add services to the container.
builder.Services.Configure<FlightDatabaseSettings>(builder.Configuration.GetSection("FlightDatabase"));
builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("JwtSetting"));
builder.Services.AddSingleton<ClientsService>();
builder.Services.AddSingleton<VolsService>();
builder.Services.AddSingleton<ReservationService>();
builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<ILog>(LogManager.GetLogger(typeof(Program)));
builder.Services.AddControllers();

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSetting:Issuer"],
        ValidAudience = builder.Configuration["JwtSetting:Audience"],
        IssuerSigningKey = new RsaSecurityKey(GetRsaPrivateKey(builder.Configuration))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("admin");
    });
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors();
}

var log = LogManager.GetLogger(typeof(Program));
log.Info("Application started.");

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

static RSA GetRsaPrivateKey(IConfiguration configuration)
{
    string privateKeyPath = @"C:\Users\rfahe\OneDrive\Documents\INTECH\API\Securiter\private_key.pem";
    string privateKeyPassword = configuration["JwtSetting:password"];
    string privateKey = File.ReadAllText(privateKeyPath);
    RSA rsa = RSA.Create();
    rsa.ImportFromEncryptedPem(privateKey, privateKeyPassword);
    return rsa;
}