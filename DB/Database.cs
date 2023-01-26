using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using MonsterCardGame.Card;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace MonsterCardGame.DB {
	internal class Database {
        private readonly string _connectionString = "";

        private static readonly string _SQL_get_all_no_table = "SELECT * FROM ";

        public Database(string connectionString) {
            this._connectionString = connectionString;
		}

        // public

        public class ReaderAndConnection : IDisposable {
            public readonly NpgsqlDataReader reader;
            public readonly NpgsqlConnection connection;

            public ReaderAndConnection(NpgsqlConnection c, NpgsqlDataReader r) {
                this.reader = r;
                this.connection = c;
            }

            public void Dispose() {
                this.reader.Dispose();
                this.connection.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        public int Count(string table) {
            using var reader = this.GetAll(table);
            int i = 0;
            while (reader.Read()) { i++; }
            return i;
        }

        // protected

        protected NpgsqlDataReader GetAll(string table) {
            return ExecuteWithDbConnection((connection) => {
                // AddWithValue does not work
                // this is only intern, so we can solve it like that
                string sqlStatement = Database._SQL_get_all_no_table + table + ";";
                using var cmd = new NpgsqlCommand(sqlStatement, connection);
                // NpgsqlParameter par = cmd.Parameters.AddWithValue("table", table);
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return reader;
            });
        }

        protected ReaderAndConnection? ExecSql(string sqlStatement, bool expectAnswer = true, string[]? keys = null, object[]? values = null) {
            if (keys != null && values != null && keys.Length != values.Length) {
                return null;
            }
            return this.ExecuteWithDbConnection((connection) => {
                using var cmd = new NpgsqlCommand(sqlStatement, connection);

                if (keys != null && values != null) {
                    for (int i = 0; i < values.Length; i++) {
                        cmd.Parameters.AddWithValue(keys[i], values[i]);
                    }
                }

                if (!expectAnswer) {
                    try {
                        cmd.ExecuteNonQuery();
                    } catch (Exception) {
                        throw;
                    }
                    connection.Close();
                    return null;
                }
                
                NpgsqlDataReader reader = cmd.ExecuteReader();
                return new ReaderAndConnection(connection, reader);
            });
        }

        public object? ExecSqlScalar(string sqlStatement, string[]? keys = null, object[]? values = null) {
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

                var ret = cmd.ExecuteScalar();
                connection.Close(); // we need to close it, or we get an exception "too many clients"
                return ret;
            });
        }

        // private

        private NpgsqlConnection Connect() {
            try {
                var connection = new NpgsqlConnection(this._connectionString);
                connection.Open();
                return connection;
            } catch (NpgsqlException) {
                // provide our own custom exception
                // throw new DataAccessFailedException("Could not connect to or initialize database", e);
                throw;
            }
        }

        private T ExecuteWithDbConnection<T>(Func<NpgsqlConnection, T> command) {
            try {
                var connection = this.Connect();

                return command(connection);
            } catch (NpgsqlException /*e*/) {

                throw;
                // throw new DataAccessFailedException("Could not connect to database", e);
            }
        }

    }
}

