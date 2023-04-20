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
    }
}
