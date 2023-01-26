using System;
using System.Data;
using Npgsql;

namespace MonsterCardGame.Card.Package {
    internal class PackageDB : DB.Database, IPackageManager {
        private static readonly string _SQL_table = "packages";
        private static readonly string _SQL_table_match = "match_packages_cards";

        // columns

        private static readonly string _SQL_column_packageId = "packageid";

        // create table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId} SERIAL PRIMARY KEY " +
            ");";
        private static readonly string _SQL_create_table_match =
            $"CREATE TABLE IF NOT EXISTS {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId} bigint, " +
                $"{CardDB._SQL_column_id} bigint " +
            ");";

        // get

        // we just want any package, so we will just use the first
        private static readonly string _SQL_get = $"SELECT * FROM {PackageDB._SQL_table} LIMIT 1;";
        private static readonly string _SQL_get_cards =
            $"SELECT * FROM {PackageDB._SQL_table_match} WHERE {PackageDB._SQL_column_packageId} = @{PackageDB._SQL_column_packageId};";

        // insert

        private static readonly string _SQL_insert =
            $"INSERT INTO {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId} " +
            ") VALUES (" +
                $"DEFAULT " +
            $") RETURNING {PackageDB._SQL_column_packageId};";
        private static readonly string _SQL_insert_match =
            $"INSERT INTO {PackageDB._SQL_table_match} (" +
                $"{PackageDB._SQL_column_packageId} " +
                $"{CardDB._SQL_column_id} " +
            ") VALUES (" +
                $"{PackageDB._SQL_column_packageId} " +
                $"{CardDB._SQL_column_id} " +
            ");";

        // remove

        private static readonly string _SQL_delete =
            $"DELETE FROM {PackageDB._SQL_table} WHERE {PackageDB._SQL_column_packageId} = @{PackageDB._SQL_column_packageId};";

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

        public PackageWithID? GetWithID() {
            int? id = this.GetId();
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

        public Package? Get() {
            return this.GetWithID()?.package;
		}

        public bool Add(Package package) {
            if (!package.IsValid()) { return false; }

            // this.ExecSql(PackageDB._SQL_insert, false);
            object? result = this.ExecSqlScalar(PackageDB._SQL_insert);
            int id = Convert.ToInt32(result);

            var keys = new string[] {
                PackageDB._SQL_column_packageId,
                CardDB._SQL_column_id
            };

            // check if card already exists -> package already exists -> fail: conflict
            for (int i = 0; i < package.Count(); i++) {
                UniqueCard? card = package.Get(i);
                if (card is null || !card.IsValid()) { /* some error - should never happen */ return false; }

                UniqueCard? already = this._cardDB.Get(card.Guid);
                if (already is not null) {
                    return false;
                }
            }

            // insert into cards + packages + match table
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
         * Get and Remove a deck
         * */
        public Package? Pop() {
            PackageWithID? pack = this.GetWithID();
            if (pack == null || !pack.HasValue) { return null; }
            this.Remove(pack.Value.id);
            return pack.Value.package;
        }

        // private functions

        private int? GetId() {
            using var info = this.ExecSql(PackageDB._SQL_get);
            if (info == null) { return null; }
            return info.reader.GetInt32(PackageDB._SQL_column_packageId);
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
    }
}

