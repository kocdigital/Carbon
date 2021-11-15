using Carbon.TimeScaleDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.TimeScaleDb
{
    public class TimeScaleDbHelper : ITimeScaleDbHelper
    {
        private readonly string _connectionString;
        private ILogger<TimeScaleDbHelper> _logger;
        public TimeScaleDbHelper(IConfiguration configuration, ILogger<TimeScaleDbHelper> logger)
        {
            _connectionString = configuration.GetConnectionString("TimeScaleDbConnectionString");
            _logger = logger;
            if (String.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("No connection string found for TimeScaleDb");
                throw new ArgumentNullException("No connection string found for TimeScaleDb");
            }


        }

        // Helper method to get a connection for the execute function
        NpgsqlConnection getConnection()
        {
            var Connection = new NpgsqlConnection(_connectionString);
            Connection.Open();
            return Connection;
        }

        //
        // Procedure - Connecting .NET to TimescaleDB:
        // Check the connection TimescaleDB and verify that the extension
        // is installed in this database
        //
        public bool CheckTimeScaleDbSupport()
        {
            // get one connection for all SQL commands below
            using (var conn = getConnection())
            {

                var sql = "SELECT default_version, comment FROM pg_available_extensions WHERE name = 'timescaledb';";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    using NpgsqlDataReader rdr = cmd.ExecuteReader();

                    if (!rdr.HasRows)
                    {
                        _logger.LogError("Missing TimescaleDB extension!");
                        conn.Close();
                        return false;
                    }

                    while (rdr.Read())
                    {
                        _logger.LogInformation("TimescaleDB Default Version: {0}\n{1}", rdr.GetString(0), rdr.GetString(1));
                    }
                    conn.Close();
                    return true;
                }
            }

        }

        public bool AddTimeScaleDbExtensionToDatabase()
        {
            // get one connection for all SQL commands below
            using (var conn = getConnection())
            {

                var sql = "CREATE EXTENSION IF NOT EXISTS timescaledb;";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        _logger.LogInformation("TimescaleDB extension enabled on the database");
                        return true;
                    }
                    catch(PostgresException pex)
                    {
                        _logger.LogWarning($"TimeScaleDb extension unable to install! Try to enable manually! - " + pex.Message);
                        return false;
                    }
                }
            }

        }

        public bool ConvertTableToTimeSeriesDb(string tableName, string timeColumnName)
        {

            using (var conn = getConnection())
            {
                using (var command = new NpgsqlCommand($"SELECT create_hypertable('{tableName}', '{timeColumnName}');", conn))
                {
                    try
                    {
                        command.ExecuteNonQuery();
                        _logger.LogInformation($"Converted the {tableName} table into a TimescaleDB hypertable!");
                        return true;
                    }
                    catch (PostgresException pex)
                    {
                        if(pex.SqlState == "TS110")
                        {
                            _logger.LogInformation($"{tableName} is already Timeserie table, no changes applied");
                            return true;
                        }
                        _logger.LogError($"Unable to Convert To TimeSerieDatabase: {pex.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Unable to Convert To TimeSerieDatabase: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
