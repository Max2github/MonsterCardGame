using System;
namespace MonsterCardGame.HTTP.Routing.Action.Command.User {
	internal class User_add : ICommand {
		private readonly MonsterCardGame.User.IUserManager _userCollection;

        public User_add(MonsterCardGame.User.IUserManager userCollection) {
			this._userCollection = userCollection;
        }

		public bool Execute(Helper.Arguments arguments, Response response) {
			object username = arguments.Get(0);
            object password = arguments.Get(1);
            // if (username is string && password is string) {
            if (arguments.IsValid(2, typeof(string), typeof(string))) {
                bool success = this._userCollection.Add(new MonsterCardGame.User.User((string)username, (string)password));
				if (success) { response.Status(Response.Status_e.CREATED_201); }
				else { response.Status(Response.Status_e.CONFLICT_409); }
				return success;
			}
			return false;
        }
	}
}

