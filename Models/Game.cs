using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlackJackApi.Models
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int? PlayerTokens { get; set; }
        public int? Bet { get; set; } = null!;
        public int? PlayerScore { get; set; } = null!;
        public int? DealerScore { get; set; } = null!;
        public int? SplitBet { get; set; } = null!;
        public int? SplitScore { get; set; } = null!;
        public void CheckWinner()
        {
            if (PlayerScore > 21 && DealerScore > 21)
            {
                PlayerTokens += Bet;
            }
            else if (DealerScore > 21)
            {
                PlayerTokens += Bet * 2;
            }
            else if (PlayerScore > 21)
            {
                return;
            }
            else if (PlayerScore > DealerScore)
            {
                PlayerTokens += Bet * 2;
            }
            else if (PlayerScore == DealerScore)
            {
                PlayerTokens += Bet;
            }
        

            // Якщо гравець не сплітував, то виходимо із методу
            if (SplitBet == null) return;

            if (SplitScore > 21 && DealerScore > 21)
            {
                PlayerTokens += SplitBet;
            }
            else if (DealerScore > 21)
            {
                PlayerTokens += SplitBet * 2;
            }
            else if (SplitScore > 21)
            {
                return;
            }
            else if (SplitScore > DealerScore)
            {
                PlayerTokens += SplitBet * 2;
            }
            else if (SplitScore == DealerScore)
            {
                PlayerTokens += SplitBet;
            }
        }
    }
}
