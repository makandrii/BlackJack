namespace BlackJackApi.Models
{
    public class Player
    {
        public int Tokens { get; set; } = 100_000;
        public int Bet { get; set; } = 0;
        public int Score { get; set; } = 0;
        public List<Card> Cards { get; set; } = new();
        public Split? Split { get; set; } = null!;
    }
}
