using System;
using MonsterCardGame.Card;
using MonsterCardGame.Card.Deck;
using MonsterCardGame.Helper;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
	internal class Deck_set : ICommand {
		private readonly MonsterCardGame.User.User _currentUser;
        private readonly IDeckManager _deckManager;
        private readonly ICardManager _cardManager;


        public Deck_set(MonsterCardGame.User.User? currentUser, IDeckManager deckManager, ICardManager cardManager) {
			this._currentUser = currentUser!;
			this._deckManager = deckManager;
			this._cardManager = cardManager;
		}

		public bool Execute(Arguments arguments, Response response) {
			if (arguments.IsValid(1, typeof(MyJsonArray))) {
				MyJsonArray idList = (MyJsonArray)arguments.Get(0);
				if (idList.Count() != 4) {
					response.Status(Response.Status_e.BAD_REQUEST_400);
					return false;
				}

				Deck deck = new(); // still invalid

				foreach (var elem in idList.Enumerate()) {
					Guid cardID = elem.GetGuid();
					UniqueCard? card = this._cardManager.Get(cardID);
					if (card is null) {
						response.Status(Response.Status_e.FORBIDDEN_403);
						return false;
					}
					deck.Next(card);
				}

				if (!deck.IsValid()) {
					response.Status(Response.Status_e.BAD_REQUEST_400);
					return false;
				}

				bool success = this._deckManager.AddOrUpdate(deck, this._currentUser.Credentials.Username);
				if (!success) {
					response.Status(Response.Status_e.INTERNAL_SERVER_ERROR_500);
					return false;
				}
				response.Status(Response.Status_e.OK_200);

				return true;
			}
			return false;
		}
	}
}

