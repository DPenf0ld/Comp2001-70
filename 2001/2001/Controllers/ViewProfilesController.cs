using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;


namespace _2001.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewProfilesController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public ViewProfilesController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET: api/<ViewProfilesController>
        [HttpGet]
        public IActionResult Get() //changes format so results are on separate lines
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection"); //change to connection string

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sqlSelect = "SELECT username, all_favorite_activities, about_me FROM CW2.CombinedData";//add sql command

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
