using BlackJackApi.Models;
using BlackJackApi.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GamesService>();
builder.Services.AddSingleton<DecksService>();

builder.Services.AddControllers();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

//app.UseSwagger();
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
