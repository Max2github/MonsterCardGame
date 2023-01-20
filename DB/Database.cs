using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace MonsterCardGame.DB {
	internal class Database {
        private string _connectionString = "";
        private NpgsqlConnection? _connection = null;

        // private readonly static string _SQL_SELECT = "SELECT * FROM @table WHERE @key=@val";

        public Database(string connectionString) {
            this._connectionString = connectionString;
		}

		public void Connect(string connectionString) {
            this._connectionString = connectionString;
            this.Connect();
        }

        // protected

        protected NpgsqlDataReader? ExecSql(string sqlStatement, bool expectAnswer = true, string[]? keys = null, object[]? values = null) {
            if (keys != null && values != null && keys.Length != values.Length) {
                return null;
            }
            return ExecuteWithDbConnection((connection) => {
                using var cmd = new NpgsqlCommand(sqlStatement, connection);

                if (keys != null && values != null) {
                    for (int i = 0; i < values.Length; i++) {
                        cmd.Parameters.AddWithValue(keys[i], values[i]);
                    }
                }

                if (!expectAnswer) {
                    try {
                        cmd.ExecuteNonQuery();
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                    return null;
                }
                
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return reader;
            });
        }

        // private

        private void Connect() {
            try {
                this._connection = new NpgsqlConnection(this._connectionString);
                this._connection.Open();
            } catch (NpgsqlException) {
                // provide our own custom exception
                // throw new DataAccessFailedException("Could not connect to or initialize database", e);
                throw;
            }
        }

        private T ExecuteWithDbConnection<T>(Func<NpgsqlConnection, T> command) {
            try {
                this.Connect();

                return command(this._connection!);
            } catch (NpgsqlException) {
                throw;
                // throw new DataAccessFailedException("Could not connect to database", e);
            }
        }

    }
}

