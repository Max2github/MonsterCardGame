using System;
using System.Text;

namespace MonsterCardGame.HTTP.Routing.Action.Command.User {
	internal class UserLogin : ICommand {
        private readonly MonsterCardGame.User.IUserManager _userCollection;

        public UserLogin(MonsterCardGame.User.IUserManager userCollection) {
            this._userCollection = userCollection;
        }

        public bool Execute(Helper.Arguments arguments, Response response) {
            object username = arguments.Get(0);
            object password = arguments.Get(1);
            // if (username is string && password is string) {
            if (arguments.IsValid(2, typeof(string), typeof(string))) {
                MonsterCardGame.User.User? user = this._userCollection.Get((string)username);
                if (user == null || (string)password != user.Credentials.Password) {
                    response.Status(Response.Status_e.UNAUTHORIZED_401);
                    return false;
                }
                user.Credentials.GenToken();
                response.Status(Response.Status_e.OK_200);

                if (Auth.AllowUnsafeToken) {
                    response.Data = Encoding.ASCII.GetBytes('\"' + Auth.GenerateUnsafeToken(user.Credentials.Username) + '\"');
                } else {
                    response.Data = Encoding.ASCII.GetBytes('\"' + user.Credentials.TokenP.Str + '\"');
                }

                return false;
            }
            return false;
        }
    }
}

