using System;
namespace MonsterCardGame.Card {
    internal class Stack {
        private readonly List<ICard> _cards = new();

        public Stack() { }
        public Stack(Deck.Deck deck) {
            UniqueCard card = deck.Next();
            while (card.IsValid()) {
                this.Push(card.Card!);
                card = deck.Next();
            }
        }

        public int Count() {
            return this._cards.Count;
        }

        public void Push(ICard card) {
            this._cards.Add(card);
        }

        public ICard? Get(int index = -1) {
            int len = this._cards.Count;
            if (len == 0) { return null; }
            if (index < 0) {
                int trueIndex = len + index;
                if (trueIndex < 0) { return null; }

                return this._cards[trueIndex];
            }
            if (index >= len) { return null; }
            return this._cards[index];
        }
        public int RandomIndex() {
            var rand = new Random();
            return rand.Next() % this.Count();
        }
        public ICard? Random() {
            return this.Get(this.RandomIndex())!;
        }


        public ICard? Pop(int index = -1) {
            int len = this._cards.Count;
            if (len == 0) { return null; }
            ICard? gotten = this.Get(index);
            if (index < 0) {
                int trueIndex = len + index; // index is negative, so we add it
                if (trueIndex < 0) { return null; }

                this._cards.RemoveAt(trueIndex);
                return gotten;
            }
            if (index >= len) { return null; }
            this._cards.RemoveAt(index);
            return gotten;
        }
        public ICard? Pop(ICard card) {
            ICard? gotten = this._cards.SingleOrDefault(x => x == card);

            bool err = !(this._cards.Remove(card));
            if (err) { /* Do something */ }
            return gotten;
        }

    }
}

