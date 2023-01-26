using System;
using System.Data;
using Npgsql;

namespace MonsterCardGame.Card.Package {
    internal class PackageDB : DB.Database, IPackageManager {
        private static readonly string _SQL_table = "packages";
        private static readonly string _SQL_table_match = "match_packages_cards";

        // columns

        private static readonly string _SQL_column_packageId = "packageid";
        // private static readonly string _SQL_column_userId = "userid";
        private static readonly string _SQL_column_username = "username";

        // create table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId} SERIAL PRIMARY KEY, " +
                $"{PackageDB._SQL_column_username } text " +
                // $"{PackageDB._SQL_column_userId} bigint, " +
                //$"FOREIGN KEY ({PackageDB._SQL_column_userId}) REFERENCES {User.UserDB._SQL_table}({User.UserDB._SQL_column_id}) " +
            ");";
        private static readonly string _SQL_create_table_match =
            $"CREATE TABLE IF NOT EXISTS {PackageDB._SQL_table_match} (" +
                $"{PackageDB._SQL_column_packageId} bigint, " +
                $"{CardDB._SQL_column_id} uuid " +
            ");";

        // get

        // we just want any package, so we will just use the first free one (without owner)
        private static readonly string _SQL_get_free =
            $"SELECT * FROM {PackageDB._SQL_table} WHERE {PackageDB._SQL_column_username} IS NULL LIMIT 1;";
        private static readonly string _SQL_get =
            $"SELECT * FROM {PackageDB._SQL_table} WHERE {PackageDB._SQL_column_username} = @{PackageDB._SQL_column_username};";
        private static readonly string _SQL_get_owner =
            $"SELECT * FROM {PackageDB._SQL_table} WHERE {PackageDB._SQL_column_packageId} = @{PackageDB._SQL_column_packageId};";
        // table match
        private static readonly string _SQL_get_cards =
            $"SELECT * FROM {PackageDB._SQL_table_match} WHERE {PackageDB._SQL_column_packageId} = @{PackageDB._SQL_column_packageId};";
        private static readonly string _SQL_get_byCard =
            $"SELECT * FROM {PackageDB._SQL_table_match} WHERE {CardDB._SQL_column_id} = @{CardDB._SQL_column_id};";

        // insert

        private static readonly string _SQL_insert =
            $"INSERT INTO {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId}, " +
                // $"{PackageDB._SQL_column_userId   }, " +
                $"{PackageDB._SQL_column_username} " +
            ") VALUES (" +
                $"DEFAULT, " +
                $"NULL " +
            $") RETURNING {PackageDB._SQL_column_packageId};";
        private static readonly string _SQL_insert_match =
            $"INSERT INTO {PackageDB._SQL_table_match} (" +
                $"{PackageDB._SQL_column_packageId}, " +
                $"{CardDB._SQL_column_id} " +
            ") VALUES (" +
                $"@{PackageDB._SQL_column_packageId}, " +
                $"@{CardDB._SQL_column_id} " +
            ");";

        // remove

        private static readonly string _SQL_delete =
            $"DELETE FROM {PackageDB._SQL_table} WHERE {PackageDB._SQL_column_packageId} = @{PackageDB._SQL_column_packageId};";

        // update

        private static readonly string _SQL_update_owner =
            $"UPDATE {PackageDB._SQL_table} SET " +
                $"{PackageDB._SQL_column_username} = @{PackageDB._SQL_column_username} " +
            $"WHERE {PackageDB._SQL_column_packageId} = @{PackageDB._SQL_column_packageId};";

        // non-static attributes

        private readonly ICardManager _cardDB;

        // public structs
        public struct PackageWithID {
            public Package package;
            public int id;
        }

        // constructor(s)

        public PackageDB(ICardManager cardDB, string connectionString) : base(connectionString) {
            this._cardDB = cardDB;
            this.ExecSql(PackageDB._SQL_create_table, false);
            this.ExecSql(PackageDB._SQL_create_table_match, false);
        }

        // public functions

        public int Count() { return this.Count(PackageDB._SQL_table); }

        public PackageWithID? GetWithID(string? username = null) {
            int? id = this.GetId(username);
            if (id == null) { return null; }
            int idHelp = Convert.ToInt32(id); // cannot convert normally from int? to int

            Package? package = this.GetPackage(idHelp);
            if (package == null) { return null; }

            PackageWithID pack = new() {
                package = package,
                id = idHelp
            };
            return pack;
        }

        public Package? Get(string? username = null) {
            return this.GetWithID(username)?.package;
		}

        public string GetOwner(int id) {
            var keys   = new string[] { PackageDB._SQL_column_packageId };
            var values = new object[] { id };
            using var info = this.ExecSql(PackageDB._SQL_get_owner, true, keys, values);
            if (info is null) { return ""; }
            if (info.reader.Read()) {
                if(info.reader.IsDBNull(PackageDB._SQL_column_username)) { return ""; }
                return info.reader.GetString(PackageDB._SQL_column_username);
            }
            return "";
        }

        public int? GetIdByCard(UniqueCard card) {
            var keys = new string[] { CardDB._SQL_column_id };
            var values = new object[] { card.Guid };
            using var info = this.ExecSql(PackageDB._SQL_get_byCard, true, keys, values);
            if (info is null) { return null; }
            if (info.reader.Read()) {
                if (info.reader.IsDBNull(PackageDB._SQL_column_username)) { return null; }
                return info.reader.GetInt32(PackageDB._SQL_column_packageId);
            }
            return null;
        }

        public bool Add(Package package) {
            if (!package.IsValid()) { return false; }

            var keys = new string[] {
                PackageDB._SQL_column_packageId,
                CardDB._SQL_column_id
            };

            // check if card already exists -> package already exists -> fail: conflict
            for (int i = 0; i < package.Count(); i++) {
                UniqueCard? card = package.Get(i);
                if (card is null || !card.IsValid()) { /* some error - should never happen */ return false; }

                UniqueCard? already = this._cardDB.Get(card.Guid);
                if (!(already is null)) {
                    return false;
                }
            }

            // insert package
            object? result = this.ExecSqlScalar(PackageDB._SQL_insert);
            int id = Convert.ToInt32(result);

            // insert into cards + match table
            for (int i = 0; i < package.Count(); i++) {
                UniqueCard? card = package.Get(i);

                this._cardDB.Add(card!);

                var values = new object[] {
                    id,
                    card!.Guid
                };
                this.ExecSql(PackageDB._SQL_insert_match, false, keys, values);
            }

            return true;
        }

        public bool Remove(int id) {
            var keys = new string[] { PackageDB._SQL_column_packageId };
            var values = new object[] { id };
            this.ExecSql(PackageDB._SQL_delete, false, keys, values);
            return true;
        }

        /**
         * Get and Remove a package
         * */
        public Package? Pop() {
            PackageWithID? pack = this.GetWithID();
            if (pack == null || !pack.HasValue) { return null; }
            this.Remove(pack.Value.id);
            return pack.Value.package;
        }

        /**
         * buy a package for a user
         * */
        public Package? Buy(string username) {
            PackageWithID? pack = this.GetWithID(); // get free package
            if (pack is null) { return null; } // no free package to buy
            this.SetOwner(username, pack.Value.id); // set owner -> can not buy annymore
            return pack.Value.package; // return package
        }

        // private functions

        private int? GetId(string? username = null) {
            var keys   = (username is null) ? null : new string[] { PackageDB._SQL_column_username };
            var values = (username is null) ? null : new object[] { username };
            string sqlStatement = (username is null) ? PackageDB._SQL_get_free : PackageDB._SQL_get;

            using ReaderAndConnection? info = this.ExecSql(sqlStatement, true, keys, values);
            if (info == null) { return null; }
            if (info.reader.Read()) {
                return info.reader.GetInt32(PackageDB._SQL_column_packageId);
            }
            return null;
        }

        private Package? GetPackage(int id) {
            var keys = new string[] { PackageDB._SQL_column_packageId };
            var values = new object[] { id };
            using var info = this.ExecSql(PackageDB._SQL_get_cards, true, keys, values);
            if (info == null) { return null; }

            Package package = new(); // still invalid

            while (info.reader.Read()) {
                Guid cardId = info.reader.GetGuid(CardDB._SQL_column_id);
                UniqueCard? card = this._cardDB.Get(cardId);
                if (card is null) { /* some error - can / should never happen */ }
                /* bool success =*/ package.Add(card!);
            }
            return package;
        }

        private void SetOwner(string username, int id) {
            var keys = new string[] {
                PackageDB._SQL_column_username,
                PackageDB._SQL_column_packageId
            };
            var values = new object[] { username, id };
            this.ExecSql(PackageDB._SQL_update_owner, false, keys, values);
        }
    }
}

