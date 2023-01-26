using System;
namespace MonsterCardGame.Card.Deck {
	internal interface IDeckManager : DB.IDB<Deck> {
        public bool Add(Deck deck, string username);
        public Deck? Get(string username);
        public bool AddOrUpdate(Deck deck, string username);
    }
}

