using System;
namespace MonsterCardGame.Card {
    public enum Type_e {
        // monsters / creatures
        monster_goblin,
        monster_wizard,
        monster_knight,
        monster_kraken,
        monster_ork,
        monster_dragon,
        monster_elf,

        // spells
        spell,
    }
    public enum Element_e {
        water,
        fire,
        normal
    }

    // interface for using the card
    public interface ICard {
        string Name { get; } // changed per class
        ushort Damage   { get; set; } // changed individually - may change it to fighting power / strenght
        // ushort Life     { get; } // changed individually - may (have to) abolish this
        Type_e Type       { get; } // changed per class
        Element_e Element { get; } // changed individually

        int AttackPower(ICard against);
    }
    // abstract class only used for DRY (Don't Repeat Yourself)
    public abstract class AbstrCard {
        public ushort Damage { get; set; } = 10;    // dummy default
        // public ushort Life { get; protected set; } = 10;    // dummy default
        public Element_e Element { get; protected set; } = Element_e.normal; // dummy default

        protected AbstrCard(Element_e element) {
            this.Element = element;
        }

        protected virtual int ElementalDmg(ICard against) {
            var damage = this.Damage;
            // effective
            if (
                (this.Element == Element_e.water  && against.Element == Element_e.fire  ) ||
                (this.Element == Element_e.fire   && against.Element == Element_e.normal) ||
                (this.Element == Element_e.normal && against.Element == Element_e.water )
            ) {
                damage *= 2;
            }
            // not effective
            if (
                (this.Element == Element_e.fire   && against.Element == Element_e.water ) ||
                (this.Element == Element_e.normal && against.Element == Element_e.fire  ) ||
                (this.Element == Element_e.water  && against.Element == Element_e.normal)
            ) {
                damage /= 2;
            }
            return damage;
        }
        
        public static bool operator ==(AbstrCard a, AbstrCard b) => (
            a.Damage == b.Damage   &&
            a.Element == b.Element
        );
        public static bool operator !=(AbstrCard a, AbstrCard b) => (
            a.Damage != b.Damage   ||
            a.Element != b.Element
        );

        // needed, because of operator overloading

        public bool Equals(AbstrCard? other) {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return this.Damage.Equals(other.Damage) && this.Element.Equals(other.Element);
        }
        public override bool Equals(object? obj) => Equals(obj as UniqueCard);

        public override int GetHashCode() {
            unchecked {
                int hashCode = this.Damage.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Element.GetHashCode();
                return hashCode;
            }
        }
    }

    // for now use a class for each type
    public class Goblin : AbstrCard, ICard {
        public string Name   { get; private set; } = "Goblin";
        public Type_e Type { get; private set; } = Type_e.monster_goblin;

        public Goblin(Element_e element) : base(element) {  }

        public int AttackPower(ICard against) {
            if (against.Type == Type_e.monster_dragon) { return 0; }
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }
    public class Wizard : AbstrCard, ICard {
        public string Name { get; private set; } = "Wizard";
        public Type_e Type { get; private set; } = Type_e.monster_wizard;

        public Wizard(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }
    public class Knight : AbstrCard, ICard {
        public string Name { get; private set; } = "Knight";
        public Type_e Type { get; private set; } = Type_e.monster_knight;

        public Knight(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            if (against.Type == Type_e.spell && against.Element == Element_e.water) { return 0; }
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }
    public class Kraken : AbstrCard, ICard {
        public string Name { get; private set; } = "Kraken";
        public Type_e Type { get; private set; } = Type_e.monster_kraken;

        public Kraken(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            // immune to spells
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }
    public class Ork : AbstrCard, ICard {
        public string Name { get; private set; } = "Ork";
        public Type_e Type { get; private set; } = Type_e.monster_ork;

        public Ork(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            // cannot deal damage to wizard, because wizards can control Orks
            if (against.Type == Type_e.monster_wizard) { return 0; }
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }
    public class Elf : AbstrCard, ICard {
        public string Name { get; private set; } = "Elf";
        public Type_e Type { get; private set; } = Type_e.monster_elf;

        public Elf(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }
    public class Dragon : AbstrCard, ICard {
        public string Name { get; private set; } = "Dragon";
        public Type_e Type { get; private set; } = Type_e.monster_dragon;

        public Dragon(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            // Fireelves can evade the dragons attack
            if (against.Element == Element_e.fire && against.Type == Type_e.monster_elf) { return 0; }
            if (against is Spell) { return this.ElementalDmg(against); }
            return this.Damage;
        }
    }

    public class Spell : AbstrCard, ICard {
        public string Name { get; private set; } = "Spell";
        public Type_e Type { get; private set; } = Type_e.monster_dragon;

        public Spell(Element_e element) : base(element) { }

        public int AttackPower(ICard against) {
            // Kraken are immune to spells
            if (against.Type == Type_e.monster_kraken) { return 0; }
            // element effective?
            return this.ElementalDmg(against);
        }
    }
}

