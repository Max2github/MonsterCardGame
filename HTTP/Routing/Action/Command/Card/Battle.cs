using System;
using MonsterCardGame.Helper;
using MonsterCardGame.HTTP;
using MonsterCardGame.Card.Battle;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
	internal class Battle : ICommand {
		private readonly Lobby _lobby;
		private readonly MonsterCardGame.User.User _user;

		public Battle(MonsterCardGame.User.User? user, Lobby lobby) {
			this._lobby = lobby;
			this._user  = user!;
		}

		public bool Execute(Arguments arguments, Response response) {
			this._lobby.Join(this._user.Credentials.Username);

            return true;

        }
	}
}

