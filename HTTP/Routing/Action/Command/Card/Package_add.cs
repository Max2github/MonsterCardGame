using System;
using MonsterCardGame.Helper;
using MonsterCardGame.Card;
using MonsterCardGame.Card.Package;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
	internal class Package_add : ICommand {
        private readonly ICardManager _cardManager;
        private readonly IPackageManager _packageManager;

        public Package_add(ICardManager cardManager, IPackageManager packageManager) {
            this._cardManager = cardManager;
            this._packageManager = packageManager;
        }

        public bool Execute(Arguments arguments, Response response) {
            if (arguments.IsValid(1, typeof(MyJsonArray))) {
                MyJsonArray json = (MyJsonArray)arguments.Get(0);

                Package package = new(); // still invalid

                int i = 0;
                foreach (var it in ((MyJsonArray)json).Enumerate()) {
                    MyJsonObject cardObj = new(it);

                    UniqueCard? card = Parser.Json(cardObj);
                    if (card is null) {
                        // sent json incorrect
                        response.Status(Response.Status_e.BAD_REQUEST_400);
                        return false;
                    }
                    bool successC = package.Add(card);
                    if (!successC) {
                        response.Status(Response.Status_e.INTERNAL_SERVER_ERROR_500);
                        return false;
                    }

                    // The following would be very inefficient: pass the json of the card to card add
                    // create Arguments for the command Card_add
                    /*
                    Arguments cardArgs = new();
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
                    }*/

                    i++;
                }

                bool success = this._packageManager.Add(package);
                if (!success) {
                    // package already exists
                    response.Status(Response.Status_e.CONFLICT_409);
                    return false;
                }

                response.Status(Response.Status_e.CREATED_201);
                return true;
            }
            return false;
        }

    }
}

