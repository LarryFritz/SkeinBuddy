using Dapper;
using Pgvector.Dapper;
using SkeinBuddy.AI;
using SkeinBuddy.DataAccess.Factories;
using SkeinBuddy.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<ConnectionFactory>();
builder.Services.AddTransient<YarnRepository>();
builder.Services.AddTransient<YarnNameEmbeddingRepository>();
builder.Services.AddTransient<XenovaEmbeddingService>();

// Map PG underscore column names to dotnet pascal case property names
DefaultTypeMap.MatchNamesWithUnderscores = true;

// Add vector type handler
SqlMapper.AddTypeHandler(new VectorTypeHandler());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
