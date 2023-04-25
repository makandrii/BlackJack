namespace BlackJackApi.Models
{
    public class Card
    {
        public string Suit { get; set; } = null!;
        public string Rank { get; set; } = null!;
        public int GetScore()
        {
            if (int.TryParse(Rank, out int result)) return result;
            if (Rank == "J" || Rank == "Q" || Rank == "K") return 10;
            return 11;
        }
    }
}