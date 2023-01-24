using System;
using MonsterCardGame.Helper;
using MonsterCardGame.Card;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
	internal class Package_add : ICommand {
        private readonly MonsterCardGame.Card.ICardManager _cardManager;

        public Package_add(MonsterCardGame.Card.ICardManager cardManager) {
            this._cardManager = cardManager;
        }

        public bool Execute(Arguments arguments, Response response) {
            if (arguments.IsValid(1, typeof(MyJsonArray))) {
                MyJsonArray json = (MyJsonArray)arguments.Get(0);

                int i = 0;
                foreach (var it in ((MyJsonArray)json).Enumerate()) {
                    // create Arguments for the command Card_add
                    Arguments cardArgs = new();
                    MyJsonObject cardObj = new(it);
                    UniqueCard? card = Parser.Json(cardObj);
                    if (card is null || !card.IsValid()) {
                        response.Status(Response.Status_e.BAD_REQUEST_400);
                        return false;
                    }
                    cardArgs.Add(card.Guid.ToString());
                    ICard icard = card.Card!;
                    cardArgs.Add(Parser.ElementToString(icard.Element) + icard.Name);
                    cardArgs.Add((long) icard.Damage);

                    // create and call command
                    var command = new Card_add(this._cardManager);
                    bool success = command.Execute(cardArgs, response);
                    if (!success) {
                        // Card_add sets the status
                        return false;
                    }

                    i++;
                }
                return true;
            }
            return false;
        }

    }
}

