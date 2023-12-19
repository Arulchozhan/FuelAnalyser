using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PaeoniaTechSpectroMeter.Database
{
    public class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public DataTable GetData(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }

        public T GetScalarValue<T>(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    var result = command.ExecuteScalar();

                    // Convert the result to the specified type
                    if (result != null && result != DBNull.Value)
                    {
                        return (T)Convert.ChangeType(result, typeof(T));
                    }

                    return default(T);
                }
            }
        }

        public bool InsertData(string query, Dictionary<string, object> parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Return true if at least one row was affected, indicating success.
                    return rowsAffected > 0;
                }
            }
        }

        public bool CheckData(string query, Dictionary<string, object> parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    int count = (int)cmd.ExecuteScalar();

                    return count > 0;// If the count is greater than 0, the record exists
                }
            }
        }

        public T ExecuteScalar<T>(string query)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (T)Convert.ChangeType(result, typeof(T));
                    }

                    return default(T);
                }
            }
        }

        internal T ExecuteScalar<T>(string countQuery, SqlParameter[] sqlParameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(countQuery, connection))
                {
                    if (sqlParameters != null)
                    {
                        command.Parameters.AddRange(sqlParameters);
                    }

                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        return (T)result;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
        }
    }
}
