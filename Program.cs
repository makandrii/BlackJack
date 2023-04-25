using BlackJackApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.WithOrigins("http://127.0.0.1:5500",
                                              "https://localhost:7024");
                      });
});

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

app.UseCors();

app.MapControllers();

app.Run();
