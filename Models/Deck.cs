using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlackJackApi.Models
{
    public class Deck
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public List<Card> Cards { get; set; } = new();
        public Deck()
        {
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            foreach (var suit in suits)
                foreach (var rank in ranks)
                    Cards.Add(new Card() { Suit = suit, Rank = rank });
        }
    }
}
