using FlightProject.Models;
using FlightProject.Services;

var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin();
            builder.AllowAnyHeader();
        });
});


// Add services to the container.
builder.Services.Configure<FlightDatabaseSettings>(
    builder.Configuration.GetSection("FlightDatabase"));
    builder.Services.AddSingleton<ClientsService>();
    builder.Services.AddSingleton<VolsService>();
    builder.Services.AddSingleton<ReservationService>();
    builder.Services.AddSingleton<JwtService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
