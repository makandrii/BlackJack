using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlackJackApi.Models
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public Dealer Dealer { get; set; } = new();
        public Player Player { get; set; } = new();
        public void CheckWinner()
        {
            if (Player.Score > 21 && Dealer.Score > 21)
            {
                Player.Tokens += Player.Bet;
            }
            else if (Dealer.Score > 21)
            {
                Player.Tokens += Player.Bet * 2;
            }
            else if (Player.Score > 21)
            {
                return;
            }
            else if (Player.Score > Dealer.Score)
            {
                Player.Tokens += Player.Bet * 2;
            }
            else if (Player.Score == Dealer.Score)
            {
                Player.Tokens += Player.Bet;
            }
        

            // Якщо гравець не сплітував, то виходимо із методу
            if (Player.Split == null) return;

            if (Player.Split.Score > 21 && Dealer.Score > 21)
            {
                Player.Tokens += Player.Split.Bet;
            }
            else if (Dealer.Score > 21)
            {
                Player.Tokens += Player.Split.Bet * 2;
            }
            else if (Player.Split.Score > 21)
            {
                return;
            }
            else if (Player.Split.Score > Dealer.Score)
            {
                Player.Tokens += Player.Split.Bet * 2;
            }
            else if (Player.Split.Score == Dealer.Score)
            {
                Player.Tokens += Player.Split.Bet;
            }
        }
    }
}
