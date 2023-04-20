﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlackJackApi.Models
{
    public class Deck
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        private int _currentCard = 0;
        public Deck()
        {
            string[] suits = { "hearts", "diamonds", "clubs", "spades" };
            string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            foreach (var suit in suits)
                foreach (var rank in ranks)
                    Cards.Add(new Card() { Suit = suit, Rank = rank });
        }
        public void ShuffleDeck()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                int j = new Random().Next(Cards.Count);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
            }
        }
        public Card DealCard() => Cards[_currentCard++];
    }
}
