using Microsoft.EntityFrameworkCore;
using OSS.API.Data;
using OSS30333.API.Data;

var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();
//builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Add services to the container.

//builder.Services.AddLogging(loggingBuilder => {
//    loggingBuilder.AddConsole();
//    loggingBuilder.AddDebug();
//});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.WriteIndented = true; // Включить отступы
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Сохранить оригинальные названия свойств
    });

//************
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
//***************


//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
await DbInitializer.SeedData(app);
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowAll"); // <- Добавьте эту строку

app.UseAuthorization();

app.MapControllers();

app.Run();

