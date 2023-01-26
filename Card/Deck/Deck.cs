using System;
namespace MonsterCardGame.Card.Deck {
	public class Deck : Helper.IValid {
		public UniqueCard Card1 { private set; get; }
        public UniqueCard Card2 { private set; get; }
        public UniqueCard Card3 { private set; get; }
        public UniqueCard Card4 { private set; get; }

        private ushort _counter_get = 1;
        private ushort _counter_set = 1;

        public Deck() {
            this.Card1 = new UniqueCard();
            this.Card2 = new UniqueCard();
            this.Card3 = new UniqueCard();
            this.Card4 = new UniqueCard();
        }
        public Deck(UniqueCard card1, UniqueCard card2, UniqueCard card3, UniqueCard card4) {
            this.Card1 = card1;
            this.Card2 = card2;
            this.Card3 = card3;
            this.Card4 = card4;
        }

        public UniqueCard Get(ushort index) {
            index %= 4; // make sure that the index is in range
            switch(index) {
                case 0: return this.Card1;
                case 1: return this.Card2;
                case 2: return this.Card3;
                case 3: return this.Card4;
            }
            // I don't like exceptions and because of the first line in this function,
            // this is never gonna happen :)
            // So this is just to make the compiler happy.
            throw new IndexOutOfRangeException();
        }

        public UniqueCard Next() {
            UniqueCard card;
            this._counter_get = (ushort)(this._counter_get % 4 + 1);
            switch (this._counter_get) {
                case 1: { card = this.Card1; break; }
                case 2: { card = this.Card2; break; }
                case 3: { card = this.Card3; break; }
                case 4: { card = this.Card4; break; }
                default: {
                    // counter error, should / can never happen
                    throw new Exception("internal error with counter in Card.Battle.Deck");
                }
            }
            this._counter_get++;
            return card;
        }

        public void Next(UniqueCard card) {
            switch (this._counter_set) {
                case 1: { this.Card1 = card; break; }
                case 2: { this.Card2 = card; break; }
                case 3: { this.Card3 = card; break; }
                case 4: { this.Card4 = card; break; }
                default: {
                    // counter error, should / can never happen
                    throw new Exception("internal error with counter in Card.Battle.Deck");
                }
            }
            this._counter_set++;
        }

        public bool IsValid() {
            return (Card1.IsValid() && Card2.IsValid() && Card3.IsValid() && Card4.IsValid());
        }

        // private
	}
}

