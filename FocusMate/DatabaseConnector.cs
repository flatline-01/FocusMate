using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace FocusMate
{
    public class DatabaseConnector
    {
        private string _connectionString = 
            "Host=localhost;Port=5432;Database=focus_mate;Username=root;Password=768867";

        public NpgsqlConnection GetConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
