using System;
namespace MonsterCardGame.HTTP.Routing.Action.Command {
	internal interface ICommand {
        bool Execute(Helper.Arguments arguments, Response response);

		static public Command.ICommand? CreateCommandByName(
			MonsterCardGame.User.User? currentUser,
			MonsterCardGame.User.IUserManager userManager,
			MonsterCardGame.Card.ICardManager cardManager,
			MonsterCardGame.Card.Package.IPackageManager packageManager,
			string name
		) {
			switch(name) {
				case "user_add"   : return new Command.User.User_add(userManager);
				case "user_get"   : return new Command.User.UserGet(currentUser, userManager);
                case "user_update": return new Command.User.UserUpdate(currentUser, userManager);
                case "user_login" : return new Command.User.UserLogin(userManager);

				case "package_add": return new Command.Card.Package_add(cardManager, packageManager);
				case "card_add"   : return new Command.Card.Card_add(cardManager);
				case "package_buy": return new Command.Card.Package_buy(currentUser, userManager, packageManager);
            }
			return null;
		}
	}
}