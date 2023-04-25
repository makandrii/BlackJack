using BlackJackApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.WithOrigins("https://makandrii.github.io",
                                              "https://makandrii-blackjack.azurewebsites.net");
                      });
});

builder.Services.AddSingleton<GamesService>();
builder.Services.AddSingleton<DecksService>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();

app.MapControllers();

app.Run();
