using System;
namespace MonsterCardGame.Card.Package {
	internal class Package : Helper.IValid {
        private readonly List<UniqueCard> _cards;
        public static int Price { get; } = 5;

        public Package() {
            this._cards = new();
        }

        public int Count() { return this._cards.Count; }

        public UniqueCard? Get(int index = 0) {
            if (index > this.Count()) { return null; }
            if (index < 0) { index = this.Count() + index; }

            return this._cards[index];
        }

        public UniqueCard? Get(Guid id) {
            return this._cards.SingleOrDefault(x => x.Guid == id);
        }

        public bool Add(UniqueCard card) {
            if (this.Get(card.Guid) is not null) { return false; }

            this._cards.Add(card);
            return true;
        }

        public bool IsValid() {
            return (this.Count() > 0);
        }
	}
}

