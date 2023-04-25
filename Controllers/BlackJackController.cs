using Microsoft.AspNetCore.Mvc;
using BlackJackApi.Models;
using BlackJackApi.Services;

namespace BlackJackApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BlackJackController : ControllerBase
{
    private readonly GamesService _gamesService;
    private readonly DecksService _decksService;
    
    public BlackJackController(GamesService gamesService, DecksService decksService)
    {
        _gamesService = gamesService;
        _decksService = decksService;
    }

    [HttpGet]
    [Route("init")]
    public async Task<Game> Initialization()
    {
        var newGame = new Game() { PlayerTokens = 100_000, PlayerScore = 0, DealerScore = 0 };
        await _gamesService.CreateAsync(newGame);

        return newGame;
    }

    [HttpGet("{id:length(24)}/deal/{whom}")]
    public async Task<ActionResult<Card>> DealCard(string id, string whom)
    {
        var deck = await _decksService.GetAsync(id);
        if (deck == null)
        {
            return NotFound();
        }

        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        var card = deck.Cards.First();
        deck.Cards.Remove(card);
        await _decksService.UpdateAsync(id, deck);

        var score = card.GetScore();
        switch (whom)
        {
            case "dealer":
                {
                    if (score == 11 && game.DealerScore + score > 21)
                        game.DealerScore += 1;
                    else
                        game.DealerScore += score;
                    break;
                }
            case "player":
                {
                    if (score == 11 && game.PlayerScore + score > 21)
                        game.PlayerScore += 1;
                    else
                        game.PlayerScore += score;
                    break;
                }
            case "split":
                {
                    if (score == 11 && game.SplitScore + score > 21)
                        game.SplitScore += 1;
                    else
                        game.SplitScore += score;
                    break;
                }
        }

        await _gamesService.UpdateAsync(id, game);

        return card;
    }

    [HttpGet("{id:length(24)}/start/{bet}")]
    public async Task<ActionResult<List<Card>>> StartGame(string id, int bet)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        if (bet > game.PlayerTokens)
        {
            return BadRequest();
        }

        var deck = new Deck() { Id = id };
        deck.ShuffleDeck();
        await _decksService.CreateAsync(deck);

        game.Bet = bet;
        game.PlayerTokens -= bet;
        await _gamesService.UpdateAsync(id, game);

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        List<Card> cards = new() 
        { 
            DealCard(id, "player").Result.Value,
            DealCard(id, "player").Result.Value,
            DealCard(id, "dealer").Result.Value
        };
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

        return cards;
    }

    [HttpGet("{id:length(24)}/end")]
    public async Task<ActionResult<Game>> EndGame(string id)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        if (game.Bet == null)
        {
            return BadRequest();
        }

        await _decksService.RemoveAsync(id);

        game.CheckWinner();

        game.SplitBet = null;
        game.SplitScore = null;
        game.Bet = null;
        game.DealerScore = 0;
        game.PlayerScore = 0;
        await _gamesService.UpdateAsync(id, game);

        return game;
    }

    [HttpGet("{id:length(24)}/double")]
    public async Task<ActionResult<Card>> Double(string id)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        if (game.Bet > game.PlayerTokens)
        {
            return BadRequest();
        }

        game.PlayerTokens -= game.Bet;
        game.Bet *= 2;
        await _gamesService.UpdateAsync(id, game);

        return await DealCard(id, "player");
    }

    [HttpGet("{id:length(24)}/split")]
    public async Task<ActionResult<List<Card>>> Split(string id)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        if (game.Bet > game.PlayerTokens)
        {
            return BadRequest();
        }

        game.PlayerTokens -= game.Bet;
        game.SplitBet = game.Bet;
        if (game.PlayerScore == 13)
        {
            game.PlayerScore--;
        }
        else
        {
            game.PlayerScore /= 2;
        }
        game.SplitScore = game.PlayerScore;
        await _gamesService.UpdateAsync(id, game);

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        List<Card> cards = new()
        {
            DealCard(id, "player").Result.Value,
            DealCard(id, "split").Result.Value
        };
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.

        return cards;
    }
}
