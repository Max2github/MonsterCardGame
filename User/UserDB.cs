using System;
using System.Data;

namespace MonsterCardGame.User {
	internal class UserDB : DB.Database, IUserManager {
        // table name
        private static readonly string _SQL_table = "users";

        // columns

        private static readonly string _SQL_column_id = "id";
        private static readonly string _SQL_column_username = "username";
        private static readonly string _SQL_column_password = "password";
        private static readonly string _SQL_column_token = "token";
        private static readonly string _SQL_column_name = "name";
        private static readonly string _SQL_column_bio = "bio";
        private static readonly string _SQL_column_image = "image";
        private static readonly string _SQL_column_admin = "admin";

        // create / initialize table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {UserDB._SQL_table} (" +
                $"{UserDB._SQL_column_id      } SERIAL PRIMARY KEY," +
                $"{UserDB._SQL_column_username} text, " +
                $"{UserDB._SQL_column_password} text, " +
                $"{UserDB._SQL_column_token   } text, " +
                $"{UserDB._SQL_column_name    } text, " +
                $"{UserDB._SQL_column_bio     } text, " +
                $"{UserDB._SQL_column_image   } text, " +
                $"{UserDB._SQL_column_admin   } boolean " +
            ");";

        // get

        private static readonly string _SQL_all_users =
            $"SELECT {UserDB._SQL_column_username} FROM {UserDB._SQL_table};";
        private static readonly string _SQL_get_user =
            $"SELECT * FROM {UserDB._SQL_table} WHERE {UserDB._SQL_column_username}=@{UserDB._SQL_column_username};";
        private static readonly string _SQL_get_user_by_token =
            $"SELECT * FROM {UserDB._SQL_table} WHERE {UserDB._SQL_column_token}=@{UserDB._SQL_column_username};";

        // insert

        private static readonly string _SQL_insert_user =
            $"INSERT INTO {UserDB._SQL_table} (" +
                $"{UserDB._SQL_column_id      }, " +
                $"{UserDB._SQL_column_username}, " +
                $"{UserDB._SQL_column_password}, " +
                $"{UserDB._SQL_column_token   }, " +
                $"{UserDB._SQL_column_name    }, " +
                $"{UserDB._SQL_column_bio     }, " +
                $"{UserDB._SQL_column_image   }, " +
                $"{UserDB._SQL_column_admin   }" +
            ") VALUES (" +
                "DEFAULT, " + 
                $"@{UserDB._SQL_column_username}, " +
                $"@{UserDB._SQL_column_password}, " +
                $"@{UserDB._SQL_column_token   }, " +
                $"@{UserDB._SQL_column_name    }, " +
                $"@{UserDB._SQL_column_bio     }, " +
                $"@{UserDB._SQL_column_image   }, " +
                $"@{UserDB._SQL_column_admin   }" +
            ");";

        // update

        private static readonly string _SQL_update_info =
            $"UPDATE {UserDB._SQL_table} SET " +
                $"{UserDB._SQL_column_name} = @{UserDB._SQL_column_name}, " +
                $"{UserDB._SQL_column_bio} = @{UserDB._SQL_column_bio}, " +
                $"{UserDB._SQL_column_image} = @{UserDB._SQL_column_image} " +
            $") WHERE {UserDB._SQL_column_username} = @{UserDB._SQL_column_username}";

        // constructor(s)

        public UserDB(string connectionString) : base(connectionString) {
            this.ExecSql(_SQL_create_table, expectAnswer: false);
        }

        // public functions

        public int Count() {
            using var reader = this.ExecSql(_SQL_all_users);
            if (reader == null) { return -1; }
            int i = 0;
            while (reader.Read()) { i++; }
            return i;
        }

        public User? Get(string username) {
            var keys = new string[] { UserDB._SQL_column_username };
            var values = new object[] { username };
            return this.GetUser(_SQL_get_user, keys, values);
        }

        public User? GetByToken(string token) {
            var keys = new string[] { UserDB._SQL_column_token };
            var values = new object[] { token };
            return this.GetUser(_SQL_get_user_by_token, keys, values);
        }
        public User? GetByToken(Credentials.Token token) {
            return this.GetByToken(token.Str);
        }

        // cmd.Prepare
        public bool Add(User user) {
            if (this.Get(user.Credentials.Username) != null) {
                // user alreay exists
                return false;
            }

            bool admin = false;
            if (user is Admin) { admin = true; }

            var keys = new string[] {
                UserDB._SQL_column_username,
                UserDB._SQL_column_password,
                UserDB._SQL_column_token,
                UserDB._SQL_column_name,
                UserDB._SQL_column_bio,
                UserDB._SQL_column_image,
                UserDB._SQL_column_admin
            };
            var values = new object[] {
                user.Credentials.Username,
                user.Credentials.Password,
                user.Credentials.TokenP.Str,
                user.Info.Name,
                user.Info.Bio,
                user.Info.Image,
                admin
            };
            /*using var reader = */this.ExecSql(UserDB._SQL_insert_user, false, keys, values);

            return true;
        }

        public bool UpdateInfo(Info userInfo, string username) {
            User? old = this.Get(username);
            if (old == null) { return false; }

            var keys = new string[] {
                // info
                UserDB._SQL_column_name,
                UserDB._SQL_column_bio,
                UserDB._SQL_column_image,
                // set where
                UserDB._SQL_column_username,
            };
            var values = new object[] {
                // info
                userInfo.Name,
                userInfo.Bio,
                userInfo.Image,
                // set where
                username
            };
            this.ExecSql(UserDB._SQL_update_info, false, keys, values);
            return true;
        }

        // private functions

        private User? GetUser(string sqlStr, string[]? keys, object[]? values) {
            using var reader = this.ExecSql(sqlStr, true, keys, values);
            if (reader == null) { return null; } // would be an internal server error, never happens
            if (reader.Read()) {
                return UserDB.ReadUser(reader);
            }
            return null;
        }

        private static User ReadUser(Npgsql.NpgsqlDataReader readingReader) {
            string usern = readingReader.GetString(UserDB._SQL_column_username);
            string pass = readingReader.GetString(UserDB._SQL_column_password);

            string name = readingReader.GetString(UserDB._SQL_column_name);
            string bio = readingReader.GetString(UserDB._SQL_column_bio);
            string image = readingReader.GetString(UserDB._SQL_column_image);

            bool admin = readingReader.GetBoolean(UserDB._SQL_column_admin);

            User user;
            if (admin) {
                user = new Admin(usern, pass, name, bio, image);
            } else {
                user = new User(usern, pass, name, bio, image);
            }

            string token = readingReader.GetString(UserDB._SQL_column_token);
            user.Credentials.SetToken(token);

            return user;
        }
    }
}

