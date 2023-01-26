using System.Text;
using MonsterCardGame.Card;
using MonsterCardGame.Card.Deck;
using MonsterCardGame.Helper;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
    internal class Deck_get : ICommand {
        private readonly IDeckManager _deckManager;
        private readonly MonsterCardGame.User.User _currentUser;

        public Deck_get(MonsterCardGame.User.User? currentUser, IDeckManager deckManager) {
            this._currentUser = currentUser!;
            this._deckManager = deckManager;
        }

        public bool Execute(Arguments arguments, Response response) {
            if (arguments.IsValid(1, typeof(MyJsonArray))) {
                Deck? deck = this._deckManager.Get(this._currentUser.Credentials.Username);
                if (deck == null) {
                    response.Status(Response.Status_e.NO_CONTENT_204);
                    return false;
                }

                response.Status(Response.Status_e.OK_200);
                response.SetHeader("Content-Type", "application/json");
                response.SetHeader("Encoding", "UTF8");
                response.Data = Encoding.UTF8.GetBytes(Parser.Json(deck!));

                return true;
            }
            return false;
        }
    }
}

