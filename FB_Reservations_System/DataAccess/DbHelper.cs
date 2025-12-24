using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FB_Reservations_System.DataAccess
{
    /// <summary>
    /// Centralized database helper for FootballDB.
    /// Uses parameterized queries and stored procedures.
    /// </summary>
    public static class DbHelper
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["FootballDB"].ConnectionString;
        }

        public static DataTable ExecuteQuery(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = commandType })
            using (var adapter = new SqlDataAdapter(cmd))
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                adapter.Fill(dt);
            }
            return dt;
        }

        public static int ExecuteNonQuery(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = commandType })
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, CommandType commandType, params SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(GetConnectionString()))
            using (var cmd = new SqlCommand(sql, conn) { CommandType = commandType })
            {
                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}