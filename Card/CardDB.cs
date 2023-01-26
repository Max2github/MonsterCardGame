using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace MonsterCardGame.Card {
	internal class CardDB : DB.Database, ICardManager {
        // table name
        internal static readonly string _SQL_table = "cards";

        // columns

        internal static readonly string _SQL_column_id      = "id";
        private static readonly string _SQL_column_type    = "type";
        private static readonly string _SQL_column_element = "element";
        private static readonly string _SQL_column_damage  = "damage";

        // create / initialize table

        private static readonly string _SQL_create_table =
            $"CREATE TABLE IF NOT EXISTS {CardDB._SQL_table} (" +
                $"{CardDB._SQL_column_id     } uuid PRIMARY KEY," +
                $"{CardDB._SQL_column_type   } bigint, " +
                $"{CardDB._SQL_column_element} bigint, " +
                $"{CardDB._SQL_column_damage } bigint" +
            ");";

        // get

        private static readonly string _SQL_get =
            $"SELECT * FROM {CardDB._SQL_table} WHERE {CardDB._SQL_column_id}=@{CardDB._SQL_column_id};";

        // insert

        private static readonly string _SQL_insert =
            $"INSERT INTO {CardDB._SQL_table} (" +
                $"{CardDB._SQL_column_id     }, " +
                $"{CardDB._SQL_column_type   }, " +
                $"{CardDB._SQL_column_element}, " +
                $"{CardDB._SQL_column_damage }" +
            ") VALUES (" +
                $"@{CardDB._SQL_column_id     }, " +
                $"@{CardDB._SQL_column_type   }, " +
                $"@{CardDB._SQL_column_element}, " +
                $"@{CardDB._SQL_column_damage }" +
            ");";

        // constructor(s)

        public CardDB(string connectionString) : base(connectionString) {
            this.ExecSql(CardDB._SQL_create_table, false);
		}

        // public functions

        public int Count() {
            return base.Count(CardDB._SQL_table);
        }

        public UniqueCard? Get(Guid guid) {
            var keys = new string[] { CardDB._SQL_column_id };
            var values = new object[] { guid };

            using var info = this.ExecSql(CardDB._SQL_get, true, keys, values);
            if (info == null) { return null; } // would be an internal server error, never happens
            if (info.reader.Read()) {
                return CardDB.ReadCard(info.reader);
            }
            return null;
        }

        public bool Add(UniqueCard card) {
            if (!card.IsValid()) {
                return false;
            }
            var keys = new string[] {
                CardDB._SQL_column_id,
                CardDB._SQL_column_type,
                CardDB._SQL_column_element,
                CardDB._SQL_column_damage
            };
            var values = new object[] {
                card.Guid,
                (int) card.Card!.Type,
                (int) card.Card.Element,
                (int) card.Card.Damage
            };
            /*using var reader = */this.ExecSql(CardDB._SQL_insert, false, keys, values);
            return true;
        }

        // private functions

        private static UniqueCard ReadCard(Npgsql.NpgsqlDataReader readingReader) {
            Guid id           =             readingReader.GetGuid(CardDB._SQL_column_id);
            Type_e type       = (Type_e)    readingReader.GetInt32(CardDB._SQL_column_type);
            Element_e element = (Element_e) readingReader.GetInt32(CardDB._SQL_column_element);
            ushort damage     = (ushort)    readingReader.GetInt32(CardDB._SQL_column_damage);

            ICard? card = Parser.ICardFromType(type, element);
            if (card == null) {
                return new UniqueCard(); // invalid
            }
            card.Damage = damage;

            return new UniqueCard(card, id);
        }
	}
}

