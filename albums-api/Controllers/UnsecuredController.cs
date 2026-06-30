using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace UnsecureApp.Controllers
{
    public class MyController
    {
        private readonly string connectionString = "";

        public string ReadFile(string userInput)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userInput);

            using (FileStream fs = File.OpenRead(userInput))
            {
                return ReadFile(fs);
            }
        }

        public string ReadFile(Stream stream)
        {
            using StreamReader reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            return reader.ReadToEnd();
        }

        public int? GetProduct(string productName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(productName);

            using SqlConnection connection = new SqlConnection(connectionString);
            using SqlCommand sqlCommand = new SqlCommand(
                "SELECT ProductId FROM Products WHERE ProductName = @ProductName",
                connection)
            {
                CommandType = CommandType.Text,
            };

            sqlCommand.Parameters.Add("@ProductName", SqlDbType.NVarChar, 200).Value = productName;

            connection.Open();
            object? result = sqlCommand.ExecuteScalar();

            return result is null or DBNull ? null : Convert.ToInt32(result);
        }

        public void GetObject()
        {
            return;
        }
    }
}