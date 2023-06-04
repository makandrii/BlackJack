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
    }
}
