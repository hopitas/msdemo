using Dapper;
using System.Data;
using System.Data.SqlClient;
using MS.model;
using MS.Utils;


namespace MS.repositories
{
    public class MessageRepository
    {
        private readonly string ConnectionString;

        public MessageRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async void InsertMessage(Message message)
        {
            Util util = new Util();
            MessageString msql = util.convertMessage(message);


            using (IDbConnection db = new SqlConnection(ConnectionString))
            {
                string sqlQuery = "Insert Into message ([key], email, attributes) Values(@Key, @Email, @Attributes)";

                int rowsAffected = db.Execute(sqlQuery, msql);
            }
        }
    }
}