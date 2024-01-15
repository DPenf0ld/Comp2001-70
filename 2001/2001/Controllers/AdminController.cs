using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;


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
            if (UserLogin.admin == true)
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
            else if (UserLogin.Loggedin == false)
            {
                return Content("Please login to an admin account");
            }
            else
            {
                return Content("Account does not have admin privileges");
            }

        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            if (UserLogin.admin == true)
            {
                string connectionString = Configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); //open connection

                    string checkUserExists = "SELECT COUNT(*) FROM CW2.User_Information WHERE user_id = @UserId";
                    using (SqlCommand checkUserExistsCommand = new SqlCommand(checkUserExists, connection))
                    {
                        checkUserExistsCommand.Parameters.AddWithValue("@UserId", id);
                        int userCount = (int)checkUserExistsCommand.ExecuteScalar();

                        if (userCount == 0)
                        {
                            return NotFound("User not found");
                        }
                    }

                    // Begin a transaction
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Use the stored procedure to delete data
                            using (SqlCommand command = new SqlCommand("CW2.delete_profile", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                // Add parameter
                                command.Parameters.AddWithValue("@user_id", id);

                                // Run stored procedure
                                command.ExecuteNonQuery();
                            }

                            // Commit the transaction if successful
                            transaction.Commit();

                            return Ok("User deleted successfully");
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if errors
                            transaction.Rollback();

                            // Return message
                            Console.WriteLine(ex.Message);

                            return BadRequest("Failed to delete user");
                        }
                    }
                }
            }
            else if (UserLogin.Loggedin == false)
            {
                return Content("Please login to an admin account");
            }
            else
            {
                return Content("Account does not have admin privileges");
            }
        }

    }
}

