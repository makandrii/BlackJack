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

        var score = GetScore(card);
        switch (whom)
        {
            case "dealer":
                {
                    game.DealerScore += score; break;
                }
            case "player":
                {
                    if (score == 11 && game.PlayerScore + score > 21)
                        game.PlayerScore += 1;
                    else
                        game.PlayerScore += score; break;
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

        var deck = new Deck() { Id = id };
        deck.ShuffleDeck();
        await _decksService.CreateAsync(deck);

        game.Bet = bet;
        game.PlayerTokens -= bet;
        await _gamesService.UpdateAsync(id, game);

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
        List<Card> cards = new List<Card>() 
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
            return NotFound();
        }

        await _decksService.RemoveAsync(id);

        CheckWinner(ref game);

        game.Bet = null;
        game.DealerScore = 0;
        game.PlayerScore = 0;
        await _gamesService.UpdateAsync(id, game);

        return game;
    }

    public int GetScore(Card card)
    {
        if (int.TryParse(card.Rank, out int result)) return result;
        if (card.Rank == "J" || card.Rank == "Q" || card.Rank == "K") return 10;
        return 11;
    }

    public void CheckWinner(ref Game game)
    {
        if (game.PlayerScore > 21 && game.DealerScore > 21)
        {
            game.PlayerTokens += game.Bet;
        }
        else if (game.DealerScore > 21)
        {
            game.PlayerTokens += game.Bet * 2;
        }
        else if (game.PlayerScore > game.DealerScore)
        {
            game.PlayerTokens += game.Bet * 2;
        }
        else if (game.PlayerScore == game.DealerScore)
        {
            game.PlayerTokens += game.Bet;
        }
    }
}
