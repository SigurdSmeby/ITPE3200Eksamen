using server.Models; // Importer AppDbContext
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Legg til tjenester til containeren.
builder.Services.AddControllers();

// Legg til CORS for å tillate kommunikasjon fra React-frontend.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")  // Tillat forespørsler fra React
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

// Legg til Entity Framework og konfigurer tilkobling til SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Legg til Swagger for API-dokumentasjon (valgfritt)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Konfigurer HTTP-request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Bruk CORS-policy
app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
