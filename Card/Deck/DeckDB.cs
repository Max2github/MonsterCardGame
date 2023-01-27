using System;
using System.Data;
using MonsterCardGame.Card.Package;

namespace MonsterCardGame.Card.Deck {
	internal class DeckDB : DB.Database, IDeckManager {
		private static readonly string _SQL_table = "deck";

        // columns

        private static readonly string _SQL_column_deckId = "deckid";
        private static readonly string _SQL_column_cardId1 = "cardid1";
        private static readonly string _SQL_column_cardId2 = "cardid2";
        private static readonly string _SQL_column_cardId3 = "cardid3";
        private static readonly string _SQL_column_cardId4 = "cardid4";
        private static readonly string _SQL_column_owner = "owner";

        // create table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {DeckDB._SQL_table} (" +
                $"{DeckDB._SQL_column_deckId} SERIAL PRIMARY KEY," +
                $"{DeckDB._SQL_column_cardId1} uuid, " +
                $"{DeckDB._SQL_column_cardId2} uuid, " +
                $"{DeckDB._SQL_column_cardId3} uuid, " +
                $"{DeckDB._SQL_column_cardId4} uuid, " +
                $"{DeckDB._SQL_column_owner} text, " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId1}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId2}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId3}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id}) , " +
                $"FOREIGN KEY({DeckDB._SQL_column_cardId4}) REFERENCES {CardDB._SQL_table}({CardDB._SQL_column_id})" +
            ");";

        // get

        private static readonly string _SQL_get =
            $"SELECT * FROM {DeckDB._SQL_table} WHERE {DeckDB._SQL_column_owner} = @{DeckDB._SQL_column_owner};";

        // insert

        private static readonly string _SQL_insert =
            $"INSERT INTO {DeckDB._SQL_table} (" +
                $"{DeckDB._SQL_column_deckId}, " +
                $"{DeckDB._SQL_column_cardId1}, " +
                $"{DeckDB._SQL_column_cardId2}, " +
                $"{DeckDB._SQL_column_cardId3}, " +
                $"{DeckDB._SQL_column_cardId4}, " +
                $"{DeckDB._SQL_column_owner}" +
            ") VALUES (" +
                $"DEFAULT, " +
                $"@{DeckDB._SQL_column_cardId1}, " +
                $"@{DeckDB._SQL_column_cardId2}, " +
                $"@{DeckDB._SQL_column_cardId3}, " +
                $"@{DeckDB._SQL_column_cardId4}, " +
                $"@{DeckDB._SQL_column_owner}" +
            ");";

        // update

        private static readonly string _SQL_update =
            $"UPDATE {DeckDB._SQL_table} SET " +
                $"{DeckDB._SQL_column_cardId1} = @{DeckDB._SQL_column_cardId1}, " +
                $"{DeckDB._SQL_column_cardId1} = @{DeckDB._SQL_column_cardId1}, " +
                $"{DeckDB._SQL_column_cardId1} = @{DeckDB._SQL_column_cardId1}, " +
                $"{DeckDB._SQL_column_cardId1} = @{DeckDB._SQL_column_cardId1} " +
            $"WHERE {DeckDB._SQL_column_deckId} = @{DeckDB._SQL_column_deckId};";

        // non-static attributes

        private readonly IPackageManager _packageDB;
        private readonly ICardManager _cardManager;

        // public structs
        public struct DeckWithID {
            public Deck deck;
            public int id;
        }

        // constructor(s)

        public DeckDB(IPackageManager packageDB, ICardManager cardManager, string connectionString) : base(connectionString) {
            this._packageDB = packageDB;
            this._cardManager = cardManager;
            this.ExecSql(DeckDB._SQL_create_table, false);
        }

		// public functions

		public int Count() { return this.Count(DeckDB._SQL_table); }

        internal DeckWithID? GetWithID(string username) {
            var keys   = new string[] { DeckDB._SQL_column_owner };
            var values = new object[] { username };
            using var info = this.ExecSql(DeckDB._SQL_get, true, keys, values);
            if (info == null) { return null; }

            Deck? deck = null;
            int id = 0;
            if (info.reader.Read()) {
                id = info.reader.GetInt32(DeckDB._SQL_column_deckId);
                deck = this.ReadDeck(info.reader);
            } else {
                return null;
            }
            DeckWithID pack = new() {
                deck = deck,
                id = id
            };
            return pack;
        }

        public Deck? Get(string username) {
            var result = this.GetWithID(username);
            if (result is null || !result.HasValue) { return null; }
            return result.Value.deck;
        }

        internal int? GetId(string username) {
            var result = this.GetWithID(username);
            if (result is null || !result.HasValue) { return null; }
            return result.Value.id;
        }

        public bool Add(Deck deck) {
            return false;
		}

        public bool Add(Deck deck, string username) {
            if (!deck.IsValid()) { return false; }

            var keys = new string[] {
                DeckDB._SQL_column_cardId1,
                DeckDB._SQL_column_cardId2,
                DeckDB._SQL_column_cardId3,
                DeckDB._SQL_column_cardId4,
                DeckDB._SQL_column_owner
            };
            var values = new object[] {
                deck.Card1.Guid,
                deck.Card2.Guid,
                deck.Card3.Guid,
                deck.Card4.Guid,
                username
            };
            this.ExecSql(DeckDB._SQL_insert, false, keys, values);

            return true;
        }

        internal bool Update(Deck deck, int id) {
            if (!deck.IsValid()) { return false; }
            var keys = new string[] {
                DeckDB._SQL_column_cardId1,
                DeckDB._SQL_column_cardId2,
                DeckDB._SQL_column_cardId3,
                DeckDB._SQL_column_cardId4
            };
            var values = new object[] {
                deck.Card1.Guid,
                deck.Card2.Guid,
                deck.Card3.Guid,
                deck.Card4.Guid
            };
            this.ExecSql(DeckDB._SQL_update, false, keys, values);
            return true;
        }

        public bool AddOrUpdate(Deck deck, string username) {
            if (!deck.IsValid()) { return false; }

            int? id = this.GetId(username);
            if (id == null) {
                return this.Add(deck, username);
            }
            int idHelp = Convert.ToInt32(id);
            return this.Update(deck, idHelp);
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
            UniqueCard? card1 = this._cardManager.Get(cardId1);
            UniqueCard? card2 = this._cardManager.Get(cardId2);
            UniqueCard? card3 = this._cardManager.Get(cardId3);
            UniqueCard? card4 = this._cardManager.Get(cardId4);

            if (card1 is null || card2 is null || card3 is null || card4 is null) {
                return new Deck(); // invalid
            }
            return new Deck(card1, card2, card3, card4);
        }
	}
}

