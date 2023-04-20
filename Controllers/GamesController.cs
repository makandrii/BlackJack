using BlackJackApi.Models;
using BlackJackApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlackJackApi.Controllers;

[ApiController]
[Route("[controller]/")]
public class GamesController : ControllerBase
{
    private readonly GamesService _gamesService;

    public GamesController(GamesService gamesService) =>
        _gamesService = gamesService;

    [HttpGet]
    public async Task<List<Game>> Get() =>
        await _gamesService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Game>> Get(string id)
    {
        var game = await _gamesService.GetAsync(id);

        if (game is null)
        {
            return NotFound();
        }

        return game;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Game newGame)
    {
        await _gamesService.CreateAsync(newGame);

        return CreatedAtAction(nameof(Get), new { id = newGame.Id }, newGame);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Game updatedGame)
    {
        var game = await _gamesService.GetAsync(id);

        if (game is null)
        {
            return NotFound();
        }

        updatedGame.Id = game.Id;

        await _gamesService.UpdateAsync(id, updatedGame);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var game = await _gamesService.GetAsync(id);

        if (game is null)
        {
            return NotFound();
        }

        await _gamesService.RemoveAsync(id);

        return NoContent();
    }
}