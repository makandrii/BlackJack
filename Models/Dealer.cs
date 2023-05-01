namespace BlackJackApi.Models
{
    public class Dealer
    {
        public int Score { get; set; } = 0;
        public List<Card> Cards { get; set; } = new();
    }
}
