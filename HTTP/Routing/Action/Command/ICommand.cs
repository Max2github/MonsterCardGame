using System;
namespace MonsterCardGame.HTTP.Routing.Action.Command {
	internal interface ICommand {
        bool Execute(Helper.Arguments arguments, Response response);

		static public Command.ICommand? CreateCommandByName(MonsterCardGame.User.IUserManager userManager, string name) {
			switch(name) {
				case "user_add"   : return new Command.User.User_add(userManager);
				case "user_get"   : return new Command.User.UserGet(userManager);
                case "user_update": return new Command.User.UserUpdate(userManager);
                case "user_login" : return new Command.User.UserLogin(userManager);
            }
			return null;
		}
	}
}