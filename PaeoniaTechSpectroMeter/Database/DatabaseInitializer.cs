using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PaeoniaTechSpectroMeter.Database
{
    public class DatabaseInitializer
    {
        private const int MaxRetries = 5;
        private const int DelaySeconds = 10;
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["FuelAnalyser"].ConnectionString;

        public DatabaseInitializer(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InitializeDatabase()
        {
            int retries = 0;
            bool connected = false;


            while (retries < MaxRetries)
            {
                try
                {
                    using(SqlConnection connection = new SqlConnection(_connectionString))
                    {
                        connection.Open();
                        connected = true;
                        break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Failed to connect the database: {e.Message}");
                    MessageBox.Show($"Failed to connect the database: {e.Message}");
                    retries++;
                    System.Threading.Thread.Sleep(DelaySeconds * 1000); //Delay before retrying
                }
            }

            if (!connected)
            {
                //Console.WriteLine($"Unable to connect to MySQL database after {MaxRetries} retries.");
                MessageBox.Show($"Unable to connect to MySQL database after {MaxRetries} retries.");
            }

        }
    }
}
