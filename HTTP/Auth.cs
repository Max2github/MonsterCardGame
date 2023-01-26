using System;
using MonsterCardGame.User;
using Newtonsoft.Json.Linq;

namespace MonsterCardGame.HTTP {
	internal class Auth {
		public enum Flags {
			None = 0,
			// position:       12345678
			Allow_all       = 0b00000001, // == 1
			Allow_admin     = 0b00000010, // == 2
			Allow_own_user  = 0b00000100, // == 3
			Allow_all_users = 0b00001000, // == 4
		}

        /// in which http header the token is
        public static string Header				{ get; set; } = "Authorization";
		/// something the stands before the token
		public static string Prefix				{ get; set; } = "Bearer ";
        /// something the stands behind the token
        public static string Suffix				{ get; set; } = "";
        /**
         * allow tokens in the form `<UnsafeToken_Prefix><username><UnsafeToken_Suffix>`
         * */
        public static bool AllowUnsafeToken		{ get; set; } = true;
		public static string UnsafeToken_Prefix { get; set; } = "";
        public static string UnsafeToken_Suffix { get; set; } = "-mtcgToken";

        private User.User? _user = null;
		private Flags _flags = Flags.None;

		public Auth() { }

		// static helper functions

		private static string GetSentToken(Request request) {
			string headerVal = request.GetHeader(Auth.Header);
            headerVal = (Auth.Prefix != "" && headerVal.Contains(Auth.Prefix)) ? headerVal.Replace(Auth.Prefix, null) : headerVal;
            headerVal = (Auth.Suffix != "" && headerVal.Contains(Auth.Suffix)) ? headerVal.Replace(Auth.Suffix, null) : headerVal;
			return headerVal;
        }

		private static string GetUsernameFromUnsafeToken(string token) {
			if (token == "") { return ""; }
			string temp = token;
			if (temp.Contains(Auth.UnsafeToken_Prefix) && Auth.UnsafeToken_Prefix != "") { temp = temp.Replace(Auth.UnsafeToken_Prefix, null); }
			if (temp.Contains(Auth.UnsafeToken_Suffix) && Auth.UnsafeToken_Suffix != "") { temp = temp.Replace(Auth.UnsafeToken_Suffix, null); }
			return (temp.Trim());
		}

		public static string GenerateUnsafeToken(string username) {
			return Auth.UnsafeToken_Prefix + username + Auth.UnsafeToken_Suffix;
		}

		public static User.User? GetUser(User.IUserManager userManager, Request request) {
			string token = Auth.GetSentToken(request);
			if (token == "") { return null; }
            if (Auth.AllowUnsafeToken) {
				User.User? user = userManager.Get(Auth.GetUsernameFromUnsafeToken(token));
				// Console.WriteLine();
				if (user != null) { return user; }
			}
			return userManager.GetByToken(token);
		}

		// Setter

		public bool SetUser(User.User? user) {
			this._user = user;
			return true;
		}

		public bool AddFlags(Flags flags) {
			this._flags |= flags;
			return true;
		}

		public bool SetFlags(Flags flags) {
			this._flags = flags;
			return true;
		}

		// check if the action is accepted

		public bool Accept(Request request) {
			// Console.WriteLine(this._flags);
			if (this._flags == Flags.Allow_all) { return true; } // allow everyone
			if (this._user is null) { return false; } // no one logged in -> deny
            if (this._flags.HasFlag(Flags.Allow_all_users)) { return true; } // allow all logged in users

			// if admin is allowed -> if logging an user is an admin -> allow
            if (this._flags.HasFlag(Flags.Allow_admin)) {
				// Console.WriteLine("Admin can");
                if (this._user is Admin) { return true; }
            }
            // if only the own user is allowed -> check if own user -> allow
            if (this._flags.HasFlag(Flags.Allow_own_user)) {
				// Console.WriteLine("Own user can");

				string token = Auth.GetSentToken(request);
                if (this._user.Credentials.TokenP.Equals(token)) { return true; }
				if (Auth.AllowUnsafeToken && Auth.GetUsernameFromUnsafeToken(token) == this._user.Credentials.Username) { return true; }
            }

			// deny every other request
			return false;
		}
	}
}

