using BlackJackApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GamesService>();
builder.Services.AddSingleton<DecksService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
