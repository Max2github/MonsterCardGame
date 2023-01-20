using System;
namespace MonsterCardGame.Card.Battle {
	internal class SingleMatch {
		private ICard _one;
        private ICard _two;

        public SingleMatch(ICard first, ICard against) {
			this._one = first;
			this._two = against;
		}

        // -1 : opponent won, 0 : none, 1 : first card won
        public short Win() {
			int att1 = this._one.AttackPower(this._two);
            int att2 = this._two.AttackPower(this._one);

			if (att1 > att2) { return  1; }; // card 1 won
            if (att1 < att2) { return -1; }; // card 2 won
			return 0; // nobody won
        }
	}
}

