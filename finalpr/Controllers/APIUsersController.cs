using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using finalpr.Models;

namespace finalpr.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class APIUsersController : ControllerBase
    {
        [HttpGet("{role}")]
        public IEnumerable<users> Get(string role)
        {
            List<users> list = new List<users>();
            var builder = WebApplication.CreateBuilder();
            string conStr = builder.Configuration.GetConnectionString("finalprContext");
            SqlConnection conn = new SqlConnection(conStr);
            string sql = " SELECT * FROM users where role= '" + role + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();

            while (reader.Read())
            {

                list.Add(new users
                {
                    name = (string)reader["name"],

                    password = (string)reader["password"],
                    Id = (int)reader["Id"],
                    role = (string)reader["role"],
                    registerDate = (DateTime)reader["registerDate"]
                });



            }

            conn.Close();
            reader.Close();
            return (list);
        }

    }
}
