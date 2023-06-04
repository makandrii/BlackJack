using BlackJackApi.Models;

namespace BlackJackApi.Extensions
{
    public static class CardExtension
    {
        public static int GetScore(this Card card)
        {
            if (int.TryParse(card.Rank, out int result)) return result;
            if (card.Rank == "J" || card.Rank == "Q" || card.Rank == "K") return 10;
            return 11;
        }
    }
}
