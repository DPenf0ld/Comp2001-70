using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _2001.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public AdminController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // GET: api/<UserInfoController>
        //get whole table
        [HttpGet]
        public IActionResult Get() //changes format so results are on separate lines
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection"); //change to connection string

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlSelect = "SELECT * FROM CW2.CombinedData";//add sql command

                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var datatable = new System.Data.DataTable();
                        datatable.Load(reader);

                        string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(datatable);

                        // Close connection
                        connection.Close();

                        return Content(jsonResult, "application/json");
                    }
                }

            }

        }
    }
}
