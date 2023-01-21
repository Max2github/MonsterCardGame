using System;
using MonsterCardGame.User;

namespace MonsterCardGame.Card {
	internal class CardDB : DB.Database, ICardManager {
        // table name
        private static readonly string _SQL_table = "cards";

        // columns

        private static readonly string _SQL_column_id      = "id";
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

        public UniqueCard? Get() {
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
	}
}

