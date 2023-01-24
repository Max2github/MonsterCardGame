using System.Text;

namespace MonsterCardGame.User {
    /**
     * A User
     * */

    internal class User : Helper.IValid {
        public Info Info { get; }
        public Credentials Credentials { get; }
        public int Money { get; private set; }

        public User(string user, string pass, string name = "", string bio = "", string image = "", int money = 20) {
            this.Info = new Info(name, bio, image);
            this.Credentials = new Credentials(user, pass);
            this.Money = money;
        }

        /**
         * substracts some money
         * */
        public bool Pay(ushort amount) {
            if (this.Money > amount) {
                this.Money -= amount;
                return true;
            }
            return false; // not enough money
        }

        public bool IsValid() { return this.Credentials.IsValid(); }
    }

    /**
     * A user with admin rights
     * */
    internal class Admin : User {
        public Admin(string user, string pass, string name = "", string bio = "", string image = "", int money = 20) :
            base(user, pass, name, bio, image, money) { }
    }

    /*
     * Info about the user -> like a profile
     * */
    internal class Info {
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Image { get; set; }

        public Info(string name = "", string bio = "", string image = "") {
            this.Name = name;
            this.Bio = bio;
            this.Image = image;
        }
    }

    /**
     * A users credentials -> important!
     * */
    internal class Credentials : Helper.IValid {
        /**
         * A token for authentification
         * */
        internal class Token  : Helper.IValid {
            public static TimeSpan timeout = new TimeSpan(0, 0, 5, 0);

            public string Str { get; }
            public DateTime TimeStamp { get; }

            public Token() { this.Str = ""; this.TimeStamp = DateTime.Now; }
            public Token(string token) {
                this.Str = token;
                this.TimeStamp =  DateTime.Now;
            }

            public bool IsValid() {
                return !(this.Str == "" || (DateTime.Now - this.TimeStamp) > timeout);
            }
            public bool Equals(Token token) {
                return (this.Str == token.Str);
            }
            public bool Equals(string token) {
                return (this.Str == token);
            }
        }

        public string Username { get; private set; }
        public string Password { get; private set; }
        public Token  TokenP   { get; private set; }

        public Credentials(string user, string pass) {
            this.Username = this.SetName(user) ? this.Username! : "";
            this.Password = pass;

            this.TokenP = new Token();
        }

        public bool SetName(string newName) {
            if (!Helper.GlobalRegex.user.IsMatch(newName)) { return false; }
            this.Username = newName;
            return true;
        }

        internal void SetToken(string token) {
            this.TokenP = new Token(token);
        }
        internal void SetToken(Credentials.Token token) {
            this.TokenP = token;
        }

        public void GenToken() {
            Random random = new Random();
            byte[] toEncode = Encoding.ASCII.GetBytes("t" + this.Username + random);
            string tokenStr = Convert.ToBase64String(toEncode);
            this.TokenP = new Token(tokenStr);
        }

        public bool IsValid() {
            // user is valid, even if token isn't (token wasn't generated)
            return !(this.Username == "" || this.Password == "");
        }
    }
}

