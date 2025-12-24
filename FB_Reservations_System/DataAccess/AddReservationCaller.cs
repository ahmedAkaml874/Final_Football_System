using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace FB_Reservations_System.DataAccess
{
    public static class AddReservationCaller
    {
        /// <summary>
        /// Calls dbo.AddReservation stored procedure.
        /// Parameter names and types exactly match the SQL procedure.
        /// </summary>
        public static void AddReservation(string customerName, string phone, int fieldId, DateTime startTime, DateTime endTime, decimal totalPrice)
        {
            var connString = ConfigurationManager.ConnectionStrings["FootballDB"].ConnectionString;
            using (var conn = new SqlConnection(connString))
            using (var cmd = new SqlCommand("dbo.AddReservation", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.NVarChar, 200) { Value = customerName });
                cmd.Parameters.Add(new SqlParameter("@Phone", SqlDbType.NVarChar, 50) { Value = (object)phone ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@FieldID", SqlDbType.Int) { Value = fieldId });
                cmd.Parameters.Add(new SqlParameter("@StartTime", SqlDbType.DateTime) { Value = startTime });
                cmd.Parameters.Add(new SqlParameter("@EndTime", SqlDbType.DateTime) { Value = endTime });
                cmd.Parameters.Add(new SqlParameter("@TotalPrice", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = totalPrice });

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}