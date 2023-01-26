using System;
namespace MonsterCardGame.HTTP.Routing.Action.Command {
	internal interface ICommand {
        bool Execute(Helper.Arguments arguments, Response response);

		static public Command.ICommand? CreateCommandByName(
			MonsterCardGame.User.User? currentUser,
			SemiGlobal glob,
            string name
		) {
			switch(name) {
				case "user_add"   : return new Command.User.User_add(glob.userManager);
				case "user_get"   : return new Command.User.UserGet(currentUser, glob.userManager);
                case "user_update": return new Command.User.UserUpdate(currentUser, glob.userManager);
                case "user_login" : return new Command.User.UserLogin(glob.userManager);

				case "package_add": return new Command.Card.Package_add(glob.cardManager, glob.packageManager);
				case "package_buy": return new Command.Card.Package_buy(currentUser, glob.userManager, glob.packageManager);

				case "deck_get"   : return new Command.Card.Deck_get(currentUser, glob.deckManager);
                case "deck_set"   : return new Command.Card.Deck_set(currentUser, glob.deckManager, glob.cardManager);
				case "battle"     : return new Command.Card.Battle(currentUser, glob.battleLobby);

                case "card_add": return new Command.Card.Card_add(glob.cardManager);
            }
			return null;
		}
	}
}