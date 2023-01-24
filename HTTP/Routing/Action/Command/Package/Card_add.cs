using System;
using MonsterCardGame.Helper;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
    internal class Card_add : ICommand {
        private readonly MonsterCardGame.Card.ICardManager _cardManager;

        public Card_add(MonsterCardGame.Card.ICardManager cardManager) {
            this._cardManager = cardManager;
        }

        public bool Execute(Arguments arguments, Response response) {
            if (arguments.IsValid(3, typeof(string), typeof(string), typeof(long))) {
                string cardId = (string) arguments.Get(0);
                string name   = (string) arguments.Get(1);
                long damage   = (long)   arguments.Get(2);

                MonsterCardGame.Card.ICard? cardi = MonsterCardGame.Card.Parser.ICardFromFullName(name);
                if (cardi is null) {
                    // name unknown
                    response.Status(Response.Status_e.BAD_REQUEST_400);
                    return false;
                }
                MonsterCardGame.Card.UniqueCard card = new(cardi, cardId);
                bool succes = this._cardManager.Add(card);
                if (!succes) {
                    return false;
                }
                response.Status(Response.Status_e.CREATED_201);
                return true;
            }
            return false;
        }
    }
}

