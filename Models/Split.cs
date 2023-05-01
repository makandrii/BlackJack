namespace BlackJackApi.Models
{
    public class Split
    {
        public int Bet { get; set; } = 0;
        public int Score { get; set; } = 0;
        public List<Card> Cards { get; set; } = new();
    }
}
