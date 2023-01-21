﻿using System;
using System.Xml.Linq;

namespace MonsterCardGame.Card {
    internal static class Parser {
        public static UniqueCard? Json(string json) {
            Helper.MyJsonObject obj = new(json);
            string id = obj.Element("Id", "");
            string fullname = obj.Element("Name", "");
            int? damage = obj.Element<int?>("Damage");
            if (id == "" || fullname == "" || damage == null) { return null; }

            Element_e element;
            string name;

            string water  = Parser.ElementToString(Element_e.water);
            string fire   = Parser.ElementToString(Element_e.fire);
            string normal = Parser.ElementToString(Element_e.normal);

            if (fullname.StartsWith(water)) { element = Element_e.water; name = fullname.Remove(0, water.Length); }
            else if (fullname.StartsWith(fire)) { element = Element_e.fire; name = fullname.Remove(0, fire.Length); }
            else if (fullname.StartsWith(normal)) { element = Element_e.normal; name = fullname.Remove(0, normal.Length); }
            else { return null; }

            ICard? card = Parser.ICardFromName(name, element);
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

        private static ICard? ICardFromName(string name, Element_e element) {
            switch (name) {
                case "Goblin": return new Goblin(element);
                case "Wizard": return new Wizard(element);
                case "Knight": return new Knight(element);
                case "Kraken": return new Wizard(element);
                case "Ork"   : return new Ork(element);
                case "Elf"   : return new Elf(element);
                case "Dragon": return new Dragon(element);
                case "Spell" : return new Spell(element);
                default: return null;
            }
        }
        private static Element_e? ElementFromName(string name) {
            switch(name) {
                case "Water" : return Element_e.water;
                case "Fire"  : return Element_e.fire;
                case "Normal": return Element_e.normal;
                default: return null;
            }
        }
        private static string ElementToString(Element_e element) {
            switch (element) {
                case Element_e.water : return "Water";
                case Element_e.fire  : return "Fire";
                case Element_e.normal: return "Normal";
                default: return "";
            }
        }
	}
}
