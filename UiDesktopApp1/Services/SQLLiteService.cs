using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UiDesktopApp1.Services
{
    public class SQLLiteService
    {
        private readonly string _connectionString;

        public SQLLiteService(string databasePath)
        {
            _connectionString = $"Data Source={databasePath};Version=3;";
            CreateTable();
        }

        private void CreateTable()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var createTableQuery = @"
                CREATE TABLE IF NOT EXISTS DefaultMessages (
                    ServiceName TEXT PRIMARY KEY,
                    XmlMessage TEXT
                );";
                using (var command = new SqliteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SaveDefaultMessage(string serviceName, string xmlMessage)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var insertQuery = @"
                INSERT OR REPLACE INTO DefaultMessages (ServiceName, XmlMessage)
                VALUES (@ServiceName, @XmlMessage);";
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", serviceName);
                    command.Parameters.AddWithValue("@XmlMessage", xmlMessage);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetDefaultMessage(string serviceName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var selectQuery = @"
                SELECT XmlMessage FROM DefaultMessages
                WHERE ServiceName = @ServiceName;";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", serviceName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.GetString(0);
                        }
                    }
                }
            }
            return null;
        }

        public void DeleteDefaultMessage(string serviceName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var deleteQuery = @"
                DELETE FROM DefaultMessages
                WHERE ServiceName = @ServiceName;";
                using (var command = new SqliteCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", serviceName);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
