using System;

namespace MonsterCardGame.Card {
    public class UniqueCard : IEquatable<UniqueCard>, Helper.IValid {
        public Guid Guid { get; }
        public ICard? Card { get; }

        // create invalid
        public UniqueCard() {
            this.Guid = Guid.Empty;
        }
        // build with existing elements -> nothing generated
        public UniqueCard(ICard card, Guid guid) {
            this.Card = card;
            this.Guid = guid;
        }
        // build with existing elements -> nothing generated
        public UniqueCard(ICard card, string guid) : this(card, Guid.Parse(guid)) { }
        // create card with new guid
        public UniqueCard(ICard card) : this(card, Guid.NewGuid()) { }

        // IValid

        public bool IsValid() {
            return !(this.Card == null || this.Guid == Guid.Empty);
        }

        // operator ==

        public static bool operator ==(UniqueCard a, UniqueCard b) {
            if (a.Guid != b.Guid) { return false; } // check GUID
            // check special cases: one or botth Cards == null
            if (a.Card == null && b.Card == null) { return false; }
            if (a.Card == null || b.Card == null) { return false; }
            // check the rest
            return (a.Card.Type == b.Card.Type);
        }
        public static bool operator !=(UniqueCard a, UniqueCard b) => (
            !(a == b)
        );

        // needed, because of operator overloading

        public bool Equals(UniqueCard? other) {
            if (Equals(other, null)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            if (!this.Guid.Equals(other.Guid)) {
                return false;
            }
            if (Equals(this.Card, null) && Equals(other.Card, null)) {
                return true;
            }
            return this.Card!.Equals(other.Card) && this.Card.Type.Equals(other.Card.Type);
        }
        public override bool Equals(object? obj) => Equals(obj as UniqueCard);

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.Guid.GetHashCode();
                int cardHash = (this.Card == null) ? 1 : this.Card.GetHashCode();
                hashCode = (hashCode * 397) ^ cardHash;
                return hashCode;
            }
        }
    }
}

