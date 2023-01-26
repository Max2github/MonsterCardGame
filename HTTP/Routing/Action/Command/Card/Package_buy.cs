using System;
using MonsterCardGame.Helper;
using MonsterCardGame.Card;
using MonsterCardGame.Card.Package;

namespace MonsterCardGame.HTTP.Routing.Action.Command.Card {
	internal class Package_buy : ICommand {
        private readonly MonsterCardGame.User.IUserManager _userManager;
        private readonly MonsterCardGame.User.User _currentUser;
        private readonly IPackageManager _packageManager;

        public Package_buy(
            MonsterCardGame.User.User? currentUser,
            MonsterCardGame.User.IUserManager userManager,
            IPackageManager packageManager
        ) {
            this._packageManager = packageManager;
            this._userManager = userManager;

            // if the user is not logged in, Auth has already kicked him / her out
            // -> no not logged in user can come here
            this._currentUser = currentUser!;
        }

        public bool Execute(Arguments arguments, Response response) {
            if (arguments.IsValid(0)) {
                // check if user can pay
                bool success = this._currentUser.Pay((ushort) Package.Price);
                if (!success) {
                    // not enough money
                    response.Status(Response.Status_e.FORBIDDEN_403);
                    return false;
                }

                // buy package
                Package? package = this._packageManager.Buy(this._currentUser.Credentials.Username);
                if (package == null) {
                    // no package to buy
                    response.Status(Response.Status_e.NOT_FOUND_404);
                    return false;
                }

                // update money in DB
                success = this._userManager.UpdateMoney(this._currentUser.Money, this._currentUser.Credentials.Username);
                if (!success) {
                    response.Status(Response.Status_e.INTERNAL_SERVER_ERROR_500);
                }

                response.Status(Response.Status_e.OK_200);
                return true;
            }
            return false;
        }

    }
}