using System;

namespace MonsterCardGame.User {
	internal class Collection : IUserManager {
		private readonly List<User> _users = new();

		public Collection() {
		}

		public int Count() {
			return this._users.Count;
		}

		public User? Get(string username) {
			return this._users.SingleOrDefault(x => x.Credentials.Username == username);
		}
        public User? GetByToken(string token) {
            return this._users.SingleOrDefault(x => x.Credentials.TokenP.Equals(token));
        }
        public User? GetByToken(Credentials.Token token) {
			return this.GetByToken(token.Str);
        }

        public bool Add(User user) {
			if (
				this.Get(user.Credentials.Username) != null ||
				this.GetByToken(user.Credentials.TokenP) != null
			) {
				/* user already there */
				return false;
			}
			this._users.Add(user);
			return true;
		}

		public bool UpdateInfo(Info userInfo, string username) {
			return false;
		}
		public bool UpdateMoney(int money, string username) {
			return false;
		}
	}
}

