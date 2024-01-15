using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;


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



        // GET api/<UserInfoController>/5
        //get data under id
        [HttpGet]
        public IActionResult Get()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            int userId = UserLogin.userloginid;

            if (userId > 0) //makes sure there is data in userId
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); //opens connection

                    // sql command
                    string sqlSelect = "SELECT * FROM CW2.CombinedData WHERE user_id = @UserId";

                    using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                    {
                        // Add my parameter
                        command.Parameters.AddWithValue("@UserId", userId);

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
                return Unauthorized("Please Login");
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
        [HttpPut]
        public IActionResult Put([FromBody] UserInformationUpdateModel updateModel)
        {
            if (updateModel == null) //checks to see if there is data
            {
                return BadRequest("Invalid update data");
            }

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            int userId = UserLogin.userloginid;

            if (userId > 0) //makes sure there is data in userId
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
                                checkUserExistsCommand.Parameters.AddWithValue("@UserId", userId);
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
                                updateColumnCommand.Parameters.AddWithValue("@UserId", userId);
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

                            return BadRequest("Unable to update user");
                        }
                    }
                }
            }
            else
            {
                return Unauthorized("Please Login");
            }
        }


        [HttpDelete]
        public IActionResult Delete()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            int userId = UserLogin.userloginid; // Get user id from UserLogin

            if (userId > 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); // Open connection

                    // Begin a transaction
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Use the stored procedure to delete data
                            using (SqlCommand command = new SqlCommand("CW2.delete_profile", connection, transaction))
                            {
                                command.CommandType = CommandType.StoredProcedure;

                                //parameter
                                command.Parameters.AddWithValue("@user_id", userId);

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
            else
            {
                return Unauthorized("Please Login");
            }
        }
    }
}

