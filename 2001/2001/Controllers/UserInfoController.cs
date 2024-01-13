using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _2001.Controllers
{
    public class UserInformationInsertModel
    {
        public string AboutMe { get; set; }
        public string Units { get; set; }
        public string ActivityTimePreference { get; set; }
        public string MarketingLanguage { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string MemberLocation { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }
        public DateTime Birthday { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public int ActivityId { get; set; }

    }

    public class UserInformationUpdateModel
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }

        // get the value of the column
        public object GetValue()
        {
            return ColumnValue;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        public IConfiguration Configuration { get; }

        public UserInfoController(IConfiguration configuration)
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

        // GET api/<UserInfoController>/5
        //get data under id
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (id == UserLogin.userloginid) {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // sql command
                    string sqlSelect = "SELECT * FROM CW2.CombinedData WHERE user_id = @UserId";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        // Add my parameter
                        command.Parameters.AddWithValue("@UserId", id);

                        // fills DataTable
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            var dataTable = new System.Data.DataTable();
                            dataAdapter.Fill(dataTable);

                            string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);

                            // Close the connection
                            connection.Close();

                            return Content(jsonResult, "application/json");
                        }
                    }
                }

            }
            else
            {
                return Unauthorized("Unauthorized access");
            }
           
        }




        // POST api/<UserInfoController>

        [HttpPost]
        public IActionResult Post([FromBody] UserInformationInsertModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid data");
            }

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Begin transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Use the stored procedure to insert data
                        using (SqlCommand command = new SqlCommand("CW2.InsertUserProfileAndInformation", connection, transaction))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            // Add parameters
                            command.Parameters.AddWithValue("@user_id", user.UserId); // Change to UserId
                            command.Parameters.AddWithValue("@username", user.Username);
                            command.Parameters.AddWithValue("@email", user.Email);
                            command.Parameters.AddWithValue("@member_location", user.MemberLocation);
                            command.Parameters.AddWithValue("@height", user.Height);
                            command.Parameters.AddWithValue("@weight", user.Weight);
                            command.Parameters.AddWithValue("@birthday", user.Birthday);
                            command.Parameters.AddWithValue("@password", user.Password);
                            command.Parameters.AddWithValue("@about_me", user.AboutMe);
                            command.Parameters.AddWithValue("@units", user.Units);
                            command.Parameters.AddWithValue("@activity_time_preference", user.ActivityTimePreference);
                            command.Parameters.AddWithValue("@marketing_language", user.MarketingLanguage);
                            command.Parameters.AddWithValue("@activity_id", user.ActivityId);


                            // Run stored procedure
                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction if successful
                        transaction.Commit();

                        return Ok("User added successfully");
                    }
                    catch (Exception ex)
                    {
                        // Rollback if error
                        transaction.Rollback();

                        // Return message
                        Console.WriteLine(ex.Message);

                        return BadRequest("Failed to add user");
                    }
                }
            }
        }






        // PUT api/<UserInfoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] UserInformationUpdateModel updateModel)
        {
            if (updateModel == null) //checks to see if there is data
            {
                return BadRequest("Invalid update data");
            }

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (id == UserLogin.userloginid)
            {

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); //open connection

                    // Begin a transaction
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if the specified user_id exists
                            string checkUserExistsQuery = "SELECT COUNT(*) FROM CW2.User_Information WHERE user_id = @UserId";
                            using (SqlCommand checkUserExistsCommand = new SqlCommand(checkUserExistsQuery, connection, transaction))
                            {
                                checkUserExistsCommand.Parameters.AddWithValue("@UserId", id);
                                int userCount = (int)checkUserExistsCommand.ExecuteScalar();

                                if (userCount == 0)
                                {
                                    return NotFound("User not found");
                                }
                            }

                            // Update the column in CW2.User_Information 
                            string updateColumnQuery = "UPDATE CW2.User_Information SET " + updateModel.ColumnName + " = @" + updateModel.ColumnName + " WHERE user_id = @UserId";
                            using (SqlCommand updateColumnCommand = new SqlCommand(updateColumnQuery, connection, transaction))
                            {
                                updateColumnCommand.Parameters.AddWithValue("@UserId", id);
                                updateColumnCommand.Parameters.AddWithValue("@" + updateModel.ColumnName, updateModel.GetValue());

                                updateColumnCommand.ExecuteNonQuery();
                            }

                            // Commit the transaction if successful
                            transaction.Commit();

                            return Ok("User information updated successfully");
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction if error
                            transaction.Rollback();

                            // return message
                            Console.WriteLine(ex.Message);

                            return BadRequest("Failed to update user information");
                        }
                    }
                }
            }
            else
            {
                return Unauthorized("Unauthorized access");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); //open connection

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


    }
}
