using System;
using MonsterCardGame.Card.Deck;
using MonsterCardGame.Card.Battle;

namespace MonsterCardGame.Card.Battle {
	internal class Battle : Helper.IValid {
		private readonly Stack _first;
        private readonly Stack _second;

		// const is automatically static
		private const int _max_rounds = 100;

        public Battle(Deck.Deck deck1, Deck.Deck deck2) {
			this._first = new Stack(deck1);
			this._second = new Stack(deck2);
		}
        public Battle(Stack deck1, Stack deck2) {
            this._first = deck1;
            this._second = deck2;
        }

        // public

        public int Start() {
			for (int i = 0; i < Battle._max_rounds;) {
				if (this._first.Count() == 0) { return -1; }  // first lost - no cards
                if (this._second.Count() == 0) { return -1; } // second lost - no cards

                int index1 = this._first.RandomIndex();
                int index2 = this._second.RandomIndex();
                ICard card1 = this._first.Get(index1)!;
                ICard card2 = this._second.Get(index2)!;

                SingleMatch match = new(card1, card2);
				int result = match.Win();
				switch(result) {
					// first won
					case  1: { Battle.MoveCard(index2, this._second, this._first); break; }
					// second won
					case -1: { Battle.MoveCard(index1, this._first, this._second); break; }

					// nobody won
					// case 0: { break; }
					default: break;
                }
			}
			return 0; // after Battle._max_rounds (100) rounds nobody won
        }

		public bool IsValid() {
			return (this._first.Count() > 0 && this._second.Count() > 0);
		}

        // private

        // move card at index (in opponents stack) from opponents stack into the winners stack
        private static bool MoveCard(int indexOpponent, Stack opponent, Stack winner) {
			var card = opponent.Pop(indexOpponent);
			if (card == null) { return false; }
			winner.Push(card);
			return true;
		}
	}
}

