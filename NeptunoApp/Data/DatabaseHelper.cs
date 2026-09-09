using System.Data;
using System.Data.SqlClient;

namespace NeptunoApp.Data
{
    public static class DatabaseHelper
    {
        private static readonly string ConnectionString =
            "Server=LAPTOP-GB67MAS4\\SQLEXPRESS;Database=NeptunoDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        // Método para ejecutar STORED PROCEDURES
        public static DataTable ExecuteQuery(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(storedProcedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        // NUEVO: Método para ejecutar CONSULTAS SQL DIRECTAS
        public static DataTable ExecuteSqlQuery(string sqlQuery)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.CommandType = CommandType.Text;

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }

        public static int ExecuteNonQuery(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(storedProcedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string storedProcedure, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand(storedProcedure, conn);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}