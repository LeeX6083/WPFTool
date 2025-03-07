using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiDesktopApp1.Models;

namespace UiDesktopApp1.Services
{
    public class ERPToolService
    {
        private readonly AppConfig _appConfig;
        private readonly string _connectionString;

        public ERPToolService(IOptions<AppConfig> appConfigOptions)
        {
            _appConfig = appConfigOptions.Value;
            // 使用配置信息
            _connectionString = $"Data Source={_appConfig.DatabaseFilePath};";
        }

        public void SaveServiceMessage(string serviceName, string xmlMessage)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var insertQuery = @"
                INSERT OR REPLACE INTO ServiceMessages (ServiceName, XmlMessage)
                VALUES (@ServiceName, @XmlMessage);";
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ServiceName", serviceName);
                    command.Parameters.AddWithValue("@XmlMessage", xmlMessage);
                    command.ExecuteNonQuery();
                }
            }
        }

        public string GetServiceMessage(string serviceName)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var selectQuery = @"
                SELECT XmlMessage FROM ServiceMessages
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
            return string.Empty;
        }

        public List<string> GetServiceNames()
        {
            List<string> serviceNames = new List<string>();
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var selectQuery = @"SELECT ServiceName FROM ServiceMessages";
                using (var command = new SqliteCommand(selectQuery, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            serviceNames.Add(reader.GetString(0));
                        }
                    }
                }
            }
            return serviceNames;
        }

        public void DeleteServiceMessages(string serviceName)
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
