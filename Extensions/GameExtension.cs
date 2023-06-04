using BlackJackApi.Models;

namespace BlackJackApi.Extensions
{
    public static class GameExtension
    {
        public static void CheckWinner(this Game game)
        {
            if (game.Player.Score > 21 && game.Dealer.Score > 21)
            {
                game.Player.Tokens += game.Player.Bet;
            }
            else if (game.Dealer.Score > 21)
            {
                game.Player.Tokens += game.Player.Bet * 2;
            }
            else if (game.Player.Score > 21)
            {
                return;
            }
            else if (game.Player.Score > game.Dealer.Score)
            {
                game.Player.Tokens += game.Player.Bet * 2;
            }
            else if (game.Player.Score == game.Dealer.Score)
            {
                game.Player.Tokens += game.Player.Bet;
            }


            // Якщо гравець не сплітував, то виходимо із методу
            if (game.Player.Split == null) return;

            if (game.Player.Split.Score > 21 && game.Dealer.Score > 21)
            {
                game.Player.Tokens += game.Player.Split.Bet;
            }
            else if (game.Dealer.Score > 21)
            {
                game.Player.Tokens += game.Player.Split.Bet * 2;
            }
            else if (game.Player.Split.Score > 21)
            {
                return;
            }
            else if (game.Player.Split.Score > game.Dealer.Score)
            {
                game.Player.Tokens += game.Player.Split.Bet * 2;
            }
            else if (game.Player.Split.Score == game.Dealer.Score)
            {
                game.Player.Tokens += game.Player.Split.Bet;
            }
        }
    }
}
