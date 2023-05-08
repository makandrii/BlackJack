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
        var newGame = new Game();
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

        if (whom == "split" && game.Player.Split == null)
        {
            return BadRequest();
        }

        var card = deck.Cards.Last();
        deck.Cards.Remove(card);
        await _decksService.UpdateAsync(id, deck);

        var score = card.GetScore();
        switch (whom)
        {
            case "dealer":
                {
                    game.Dealer.Cards.Add(card);
                    while (game.Dealer.Score + score > 21
                        && game.Dealer.Cards.Any(element => element.Rank == "A"))
                    {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                        game.Dealer.Cards.Find(element => element.Rank == "A").Rank = "1";
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                        game.Dealer.Score -= 10;
                    }
                    game.Dealer.Score += score;
                    break;
                }
            case "player":
                {
                    game.Player.Cards.Add(card);
                    while (game.Player.Score + score > 21
                        && game.Player.Cards.Any(element => element.Rank == "A"))
                    {
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                        game.Player.Cards.Find(element => element.Rank == "A").Rank = "1";
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                        game.Player.Score -= 10;
                    }
                    game.Player.Score += score;
                    break;
                }
            case "split":
                { 
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                    game.Player.Split.Cards.Add(card);
                    while (game.Player.Split.Score + score > 21
                        && game.Player.Split.Cards.Any(element => element.Rank == "A"))
                    {
                        game.Player.Split.Cards.Find(element => element.Rank == "A").Rank = "1";
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                        game.Player.Split.Score -= 10;
                    }
                    game.Player.Split.Score += score;
                    break;
                }
            default:
                {
                    return BadRequest();
                }
        }

        await _gamesService.UpdateAsync(id, game);

        if (card.Rank == "1")
        {
            card.Rank = "A";
        }
        return card;
    }

    [HttpGet("{id:length(24)}/start/{bet}")]
    public async Task<ActionResult<IEnumerable<Card>>> StartGame(string id, int bet)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }
        if (bet > game.Player.Tokens)
        {
            return BadRequest();
        }

        var deck = new Deck() { Id = id };
        deck.ShuffleDeck();
        await _decksService.CreateAsync(deck);

        game.Player.Bet = bet;
        game.Player.Tokens -= bet;
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

        if (game.Player.Bet == 0)
        {
            return BadRequest();
        }

        await _decksService.RemoveAsync(id);

        game.CheckWinner();

        game.Dealer.Score = 0;
        game.Player.Score = 0;
        game.Player.Bet = 0;
        game.Player.Cards = new();
        game.Dealer.Cards = new();
        if (game.Player.Split != null)
        {
            game.Player.Split = null;
        }
        await _gamesService.UpdateAsync(id, game);

        return game;
    }

    [HttpGet("{id:length(24)}/double/{whom}")]
    public async Task<ActionResult<Card>> Double(string id, string whom)
    {

        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        switch (whom)
        {
            case "player":
                {
                    if (game.Player.Bet > game.Player.Tokens)
                    {
                        return BadRequest();
                    }

                    game.Player.Tokens -= game.Player.Bet;
                    game.Player.Bet *= 2;
                    await _gamesService.UpdateAsync(id, game);

                    return await DealCard(id, "player");
                }
            case "split":
                {
                    if (game.Player.Split == null
                        || game.Player.Split.Bet > game.Player.Tokens)
                    {
                        return BadRequest();
                    }

                    game.Player.Tokens -= game.Player.Split.Bet;
                    game.Player.Split.Bet *= 2;
                    await _gamesService.UpdateAsync(id, game);

                    return await DealCard(id, "split");
                }
            default:
                {
                    return BadRequest();
                }
        }
    }

    [HttpGet("{id:length(24)}/split")]
    public async Task<ActionResult<IEnumerable<Card>>> Split(string id)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }

        if (game.Player.Bet > game.Player.Tokens || game.Player.Cards.Count != 2)
        {
            return BadRequest();
        }

        game.Player.Tokens -= game.Player.Bet;
        game.Player.Split = new()
        {
            Bet = game.Player.Bet
        };

        var card = game.Player.Cards[0];

        game.Player.Cards.Remove(card);
        game.Player.Score -= card.GetScore();

        if (card.Rank == "1")
        {
            card.Rank = "A";
        }

        game.Player.Split.Cards.Add(card);
        game.Player.Split.Score += card.GetScore();

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

    [HttpGet("{id:length(24)}/stay")]
    public async Task<ActionResult<IEnumerable<Card>>> Stay(string id)
    {
        var game = await _gamesService.GetAsync(id);
        if (game == null)
        {
            return NotFound();
        }
    
        var result = new List<Card>();
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
        while (game.Dealer.Score < 17 && game.Dealer.Cards.Count < 5)
        {
#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            result.Add(DealCard(id, "dealer").Result.Value);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
            game = await _gamesService.GetAsync(id);
        }
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

        return result;
    }

    [HttpDelete("wipe")]
    public async Task<ActionResult> WipeDB()
    {
        await _decksService.WipeAsync();
        await _gamesService.WipeAsync();
        return NoContent();
    }
}
