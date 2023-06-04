using BlackJackApi.Models;

namespace BlackJackApi.Extensions
{
    public static class DeckExtansion
    {
        public static void ShuffleDeck(this Deck deck)
        {
            for (int i = 0; i < deck.Cards.Count; i++)
            {
                int j = new Random().Next(deck.Cards.Count);
                (deck.Cards[i], deck.Cards[j]) = (deck.Cards[j], deck.Cards[i]);
            }
        }
    }
}
