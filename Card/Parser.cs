using System;
using System.Xml.Linq;
using MonsterCardGame.Helper;

namespace MonsterCardGame.Card {
    public static class Parser {
        public static UniqueCard? Json(string json) {
            MyJsonObject obj = new(json);
            return Parser.Json(obj);
        }
        public static UniqueCard? Json(MyJsonObject json) {
            string id       = json.Element("Id", "");
            string fullname = json.Element("Name", "");
            double? damage     = json.Element<double?>("Damage");
            if (id == "" || fullname == "" || damage == null) { return null; }

            ICard? card = Parser.ICardFromFullName(fullname);
            if (card == null) { return null; }
            return new UniqueCard(card, id);
        }

        public static string Json(UniqueCard card) {
            if (card.IsValid()) { return "invalid"; }
            string json = "{";
            json += "\"Id\":\"" + card.Guid + "\",";
            json += "\"Name\":\"" + Parser.ElementToString(card.Card!.Element) + card.Card.Name + "\",";
            json += "\"Damage\":\"" + card.Card.Damage + "\"";
            json += "}";
            return json;
        }

        public static ICard? ICardFromFullName(string fullname) {
            Element_e element;
            string name;

            string water = Parser.ElementToString(Element_e.water);
            string fire = Parser.ElementToString(Element_e.fire);
            string normal = Parser.ElementToString(Element_e.normal);

            if (fullname.StartsWith(water)) { element = Element_e.water; name = fullname.Remove(0, water.Length); }
            else if (fullname.StartsWith(fire)) { element = Element_e.fire; name = fullname.Remove(0, fire.Length); }
            else if (fullname.StartsWith(normal)) { element = Element_e.normal; name = fullname.Remove(0, normal.Length); }
            else { return Parser.ICardFromName(fullname, Element_e.normal); }

            return Parser.ICardFromName(name, element);
        }

        public static ICard? ICardFromName(string name, Element_e element) {
            switch (name) {
                case "Goblin": return new Goblin(element);
                case "Wizard": return new Wizard(element);
                case "Knight": return new Knight(element);
                case "Kraken": return new Kraken(element);
                case "Ork"   : return new Ork(element);
                case "Elf"   : return new Elf(element);
                case "Dragon": return new Dragon(element);
                case "Spell" : return new Spell(element);
                default: return null;
            }
        }
        public static ICard? ICardFromType(Type_e type, Element_e element) {
            switch (type) {
                case Type_e.monster_goblin: return new Goblin(element);
                case Type_e.monster_wizard: return new Wizard(element);
                case Type_e.monster_knight: return new Knight(element);
                case Type_e.monster_kraken: return new Kraken(element);
                case Type_e.monster_ork   : return new Ork(element);
                case Type_e.monster_elf   : return new Elf(element);
                case Type_e.monster_dragon: return new Dragon(element);
                case Type_e.spell         : return new Spell(element);
                default: return null;
            }
        }
        public static Element_e? ElementFromName(string name) {
            switch(name) {
                case "Water" : return Element_e.water;
                case "Fire"  : return Element_e.fire;
                case "Normal": return Element_e.normal;
                default: return null;
            }
        }
        public static string ElementToString(Element_e element) {
            switch (element) {
                case Element_e.water : return "Water";
                case Element_e.fire  : return "Fire";
                case Element_e.normal: return "Regular";
                default: return "";
            }
        }

        public static string Json(Deck.Deck deck) {
            string json = "[";

            UniqueCard card = deck.Next();
            while (true) {
                json += Parser.Json(card);

                card = deck.Next();
                if (!card.IsValid()) {
                    break;
                } else {
                    json += ", ";
                }
            }

            json += "]";
            return json;
        } 

        internal static string GetOwner(Package.IPackageManager packageManager, UniqueCard card) {
            int? id = packageManager.GetIdByCard(card);
            if (id == null) { return ""; } // package does not exist
            int idHelp = Convert.ToInt32(id);
            return packageManager.GetOwner(idHelp);
        }
	}
}

