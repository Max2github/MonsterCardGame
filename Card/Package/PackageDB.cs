using System;
using System.Data;

namespace MonsterCardGame.Card.Package {
	internal class PackageDB : DB.Database, IPackageManager {
		private static readonly string _SQL_table = "packages";

        // columns

        private static readonly string _SQL_column_packageId = "packageid";
        private static readonly string _SQL_column_cardId1 = "cardid1";
        private static readonly string _SQL_column_cardId2 = "cardid2";
        private static readonly string _SQL_column_cardId3 = "cardid3";
        private static readonly string _SQL_column_cardId4 = "cardid4";

        // create table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId} SERIAL PRIMARY KEY," +
                $"{PackageDB._SQL_column_cardId1} uuid, " +
                $"{PackageDB._SQL_column_cardId2} uuid, " +
                $"{PackageDB._SQL_column_cardId3} uuid, " +
                $"{PackageDB._SQL_column_cardId4} uuid, " +
                $"FOREIGN KEY({PackageDB._SQL_column_cardId1}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({PackageDB._SQL_column_cardId2}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({PackageDB._SQL_column_cardId3}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({PackageDB._SQL_column_cardId4}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
            ");";

        // get

        // we just want any package, so we will just use the first
        private static readonly string _SQL_get = $"SELECT * FROM {PackageDB._SQL_table} LIMIT 1;";

        // insert

        private static readonly string _SQL_insert =
            $"INSERT INTO {PackageDB._SQL_table} (" +
                $"{PackageDB._SQL_column_packageId}, " +
                $"{PackageDB._SQL_column_cardId1}, " +
                $"{PackageDB._SQL_column_cardId2}, " +
                $"{PackageDB._SQL_column_cardId3}, " +
                $"{PackageDB._SQL_column_cardId4}" +
            ") VALUES (" +
                $"DEFAULT, " +
                $"@{PackageDB._SQL_column_cardId1}, " +
                $"@{PackageDB._SQL_column_cardId2}, " +
                $"@{PackageDB._SQL_column_cardId3}, " +
                $"@{PackageDB._SQL_column_cardId4}" +
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
        }

		// public functions

		public int Count() { return this.Count(PackageDB._SQL_table); }

        public PackageWithID? GetWithID() {
            using var reader = this.ExecSql(PackageDB._SQL_get);
            if (reader == null) { return null; }
            Package? package = null;
            int id = 0;
            if (reader.Read()) {
                id = reader.GetInt32(PackageDB._SQL_column_packageId);
                package = this.ReadPackage(reader);
            } else {
                return null;
            }
            PackageWithID pack = new();
            pack.package = package;
            pack.id = id;
            return pack;
        }

        public Package? Get() {
            using var reader = this.ExecSql(PackageDB._SQL_get);
            if (reader == null) { return null; }
            if (reader.Read()) {
                return this.ReadPackage(reader);
            }
			return null;
		}

        public bool Add(Package package) {
            if (!package.IsValid()) { return false; }

            var keys = new string[] {
                PackageDB._SQL_column_cardId1,
                PackageDB._SQL_column_cardId2,
                PackageDB._SQL_column_cardId3,
                PackageDB._SQL_column_cardId4
            };
            var values = new object[] {
                package.Card1.Guid,
                package.Card2.Guid,
                package.Card3.Guid,
                package.Card4.Guid
            };
            this.ExecSql(PackageDB._SQL_insert, false, keys, values);

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

        // private functions

        private Package ReadPackage(Npgsql.NpgsqlDataReader readingReader) {
            Guid cardId1 = readingReader.GetGuid(PackageDB._SQL_column_cardId1);
            Guid cardId2 = readingReader.GetGuid(PackageDB._SQL_column_cardId2);
            Guid cardId3 = readingReader.GetGuid(PackageDB._SQL_column_cardId3);
            Guid cardId4 = readingReader.GetGuid(PackageDB._SQL_column_cardId4);
            UniqueCard? card1 = this._cardDB.Get(cardId1);
            UniqueCard? card2 = this._cardDB.Get(cardId2);
            UniqueCard? card3 = this._cardDB.Get(cardId3);
            UniqueCard? card4 = this._cardDB.Get(cardId4);

            if (card1 is null || card2 is null || card3 is null || card4 is null) {
                return new Package(); // invalid
            }
            return new Package(card1, card2, card3, card4);
        }
	}
}

