using System;
using System.Data;

namespace MonsterCardGame.Card.Deck {
	internal class DeckDB : DB.Database, IDeckManager {
		private static readonly string _SQL_table = "packages";

        // columns

        private static readonly string _SQL_column_deckId = "packageid";
        private static readonly string _SQL_column_cardId1 = "cardid1";
        private static readonly string _SQL_column_cardId2 = "cardid2";
        private static readonly string _SQL_column_cardId3 = "cardid3";
        private static readonly string _SQL_column_cardId4 = "cardid4";

        // create table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {DeckDB._SQL_table} (" +
                $"{DeckDB._SQL_column_deckId} SERIAL PRIMARY KEY," +
                $"{DeckDB._SQL_column_cardId1} uuid, " +
                $"{DeckDB._SQL_column_cardId2} uuid, " +
                $"{DeckDB._SQL_column_cardId3} uuid, " +
                $"{DeckDB._SQL_column_cardId4} uuid, " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId1}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId2}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId3}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId4}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
            ");";

        // get

        // we just want any package, so we will just use the first
        private static readonly string _SQL_get = $"SELECT * FROM {DeckDB._SQL_table} LIMIT 1;";

        // insert

        private static readonly string _SQL_insert =
            $"INSERT INTO {DeckDB._SQL_table} (" +
                $"{DeckDB._SQL_column_deckId}, " +
                $"{DeckDB._SQL_column_cardId1}, " +
                $"{DeckDB._SQL_column_cardId2}, " +
                $"{DeckDB._SQL_column_cardId3}, " +
                $"{DeckDB._SQL_column_cardId4}" +
            ") VALUES (" +
                $"DEFAULT, " +
                $"@{DeckDB._SQL_column_cardId1}, " +
                $"@{DeckDB._SQL_column_cardId2}, " +
                $"@{DeckDB._SQL_column_cardId3}, " +
                $"@{DeckDB._SQL_column_cardId4}" +
            ");";

        // update

        private static readonly string _SQL_update = "";
            //$"DELETE FROM {DeckDB._SQL_table} WHERE {DeckDB._SQL_column_deckId} = @{DeckDB._SQL_column_deckId};";

        // non-static attributes

        private readonly ICardManager _cardDB;

        // public structs
        public struct DeckWithID {
            public Deck deck;
            public int id;
        }

        // constructor(s)

        public DeckDB(ICardManager cardDB, string connectionString) : base(connectionString) {
            this._cardDB = cardDB;
            this.ExecSql(DeckDB._SQL_create_table, false);
        }

		// public functions

		public int Count() { return this.Count(DeckDB._SQL_table); }

        /*public DeckWithID? GetWithID() {
            using var reader = this.ExecSql(DeckDB._SQL_get);
            if (reader == null) { return null; }
            Deck? deck = null;
            int id = 0;
            if (reader.Read()) {
                id = reader.GetInt32(DeckDB._SQL_column_deckId);
                deck = this.ReadDeck(reader);
            } else {
                return null;
            }
            DeckWithID pack = new() {
                deck = deck,
                id = id
            };
            return pack;
        }*/

        /*public Deck? Get() {
            using var reader = this.ExecSql(DeckDB._SQL_get);
            if (reader == null) { return null; }
            if (reader.Read()) {
                return this.ReadDeck(reader);
            }
			return null;
		}*/

        public bool Add(Deck package) {
            if (!package.IsValid()) { return false; }

            var keys = new string[] {
                DeckDB._SQL_column_cardId1,
                DeckDB._SQL_column_cardId2,
                DeckDB._SQL_column_cardId3,
                DeckDB._SQL_column_cardId4
            };
            var values = new object[] {
                package.Card1.Guid,
                package.Card2.Guid,
                package.Card3.Guid,
                package.Card4.Guid
            };
            this.ExecSql(DeckDB._SQL_insert, false, keys, values);

			return true;
		}

        /*public bool Remove(int id) {
            var keys = new string[] { DeckDB._SQL_column_packageId };
            var values = new object[] { id };
            this.ExecSql(DeckDB._SQL_delete, false, keys, values);
            return true;
        }*/

        /**
         * Get and Remove a deck
         * */
        /*public Package? Pop() {
            PackageWithID? pack = this.GetWithID();
            if (pack == null || !pack.HasValue) { return null; }
            this.Remove(pack.Value.id);
            return pack.Value.package;
        }*/

        // private functions

        private Deck ReadDeck(Npgsql.NpgsqlDataReader readingReader) {
            Guid cardId1 = readingReader.GetGuid(DeckDB._SQL_column_cardId1);
            Guid cardId2 = readingReader.GetGuid(DeckDB._SQL_column_cardId2);
            Guid cardId3 = readingReader.GetGuid(DeckDB._SQL_column_cardId3);
            Guid cardId4 = readingReader.GetGuid(DeckDB._SQL_column_cardId4);
            UniqueCard? card1 = this._cardDB.Get(cardId1);
            UniqueCard? card2 = this._cardDB.Get(cardId2);
            UniqueCard? card3 = this._cardDB.Get(cardId3);
            UniqueCard? card4 = this._cardDB.Get(cardId4);

            if (card1 is null || card2 is null || card3 is null || card4 is null) {
                return new Deck(); // invalid
            }
            return new Deck(card1, card2, card3, card4);
        }
	}
}

