using System;
using System.Text;

namespace MonsterCardGame.HTTP.Routing.Action.Command.User {
	internal class UserUpdate : ICommand {
		private readonly MonsterCardGame.User.IUserManager _userCollection;

        public UserUpdate(MonsterCardGame.User.IUserManager userCollection) {
			this._userCollection = userCollection;
        }

		public bool Execute(Helper.Arguments arguments, Response response) {
            object username = arguments.Get(0);
            object newName = arguments.Get(1);
            object bio	   = arguments.Get(2);
            object image   = arguments.Get(3);

            if (arguments.IsValid(4, typeof(string), typeof(string), typeof(string), typeof(string))) {
                MonsterCardGame.User.Info userInfo = new MonsterCardGame.User.Info();
                userInfo.Name = (string)newName;
                userInfo.Bio = (string)bio;
                userInfo.Image = (string)image;

				bool success = this._userCollection.UpdateInfo(userInfo, (string) username);
                if (!success) {
                    response.Status(Response.Status_e.NOT_FOUND_404);
                    return false;
                }

				response.Status(Response.Status_e.OK_200);
				return true;
			}
			return false;
        }
	}
}

