using System;
using System.Text;

namespace MonsterCardGame.HTTP.Routing.Action.Command.User {
	internal class UserGet : ICommand {
		private readonly MonsterCardGame.User.IUserManager _userCollection;

        public UserGet(MonsterCardGame.User.IUserManager userCollection) {
			this._userCollection = userCollection;
        }

		public bool Execute(Helper.Arguments arguments, Response response) {
			object username = arguments.Get(0);
            if (arguments.IsValid(1, typeof(string))) {
                MonsterCardGame.User.Info? userInfo = this._userCollection.Get((string) username)?.Info;
				if (userInfo == null) {
					response.Status(Response.Status_e.NOT_FOUND_404);
					return false;
				}
                response.Data = Encoding.UTF8.GetBytes($"{{ \"Name\":{userInfo.Name}, \"Bio\":\"{userInfo.Bio}\", \"Image\":\"{userInfo.Image}\" }}");

				response.Status(Response.Status_e.OK_200);
				response.SetHeader("Encoding", "UTF-8");
				response.SetHeader("Content-Type", "application/json");
				response.SetHeader("Content-Length", $"{response.Data.Length}");
				return true;
			}
			return false;
        }
	}
}

