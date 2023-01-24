using System;
using MonsterCardGame.Helper;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
	internal class Package_get : ICommand {
        private readonly MonsterCardGame.Card.ICardManager _cardManager;

        public Package_get(MonsterCardGame.Card.ICardManager cardManager) {
            this._cardManager = cardManager;
        }

        public bool Execute(Arguments arguments, Response response) {

            return false;
        }

    }
}

