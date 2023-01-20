using System;
namespace MonsterCardGame.Card.Battle {
	internal class Deck : Helper.IValid {
		public readonly UniqueCard card1;
        public readonly UniqueCard card2;
        public readonly UniqueCard card3;
        public readonly UniqueCard card4;

        private ushort _counter = 1;

		public Deck(UniqueCard c1, UniqueCard c2, UniqueCard c3, UniqueCard c4) {
			this.card1 = c1;
            this.card2 = c2;
            this.card3 = c3;
            this.card4 = c4;
        }

        public UniqueCard Next() {
            UniqueCard card;
            this._counter = (ushort) (this._counter % 4 + 1);
            switch(this._counter) {
                case 1: { card = this.card1; break; }
                case 2: { card = this.card2; break; }
                case 3: { card = this.card3; break; }
                case 4: { card = this.card4; break; }
                default: {
                    // counter error, should never happen
                    throw new Exception("internal error with counter in Card.Battle.Deck");
                }
            }
            this._counter++;
            return card;
        }

        public bool IsValid() {
            return true;
        }
	}
}

