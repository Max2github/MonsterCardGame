using System;
namespace MonsterCardGame.Card.Package {
	internal class Package : Helper.IValid {
		public UniqueCard Card1 { get; }
        public UniqueCard Card2 { get; }
        public UniqueCard Card3 { get; }
        public UniqueCard Card4 { get; }

        public Package() {
            this.Card1 = new UniqueCard();
            this.Card2 = new UniqueCard();
            this.Card3 = new UniqueCard();
            this.Card4 = new UniqueCard();
        }
        public Package(UniqueCard card1, UniqueCard card2, UniqueCard card3, UniqueCard card4) {
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

        public bool IsValid() {
            return (Card1.IsValid() && Card2.IsValid() && Card3.IsValid() && Card4.IsValid());
        }
	}
}

