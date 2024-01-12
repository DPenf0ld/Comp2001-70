using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.PortableExecutable;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _2001.Controllers
{
    public class UserInformationUpdateModel
    {
        public string ColumnName { get; set; }
        public string ColumnValue { get; set; }

        // Method to get the value of the column
        public object GetValue()
        {
            return ColumnValue;
        }
    }

    public class UserInformationModel
    {
        //CW2.User_Information
        public int user_id { get; set; }
        public string email { get; set; }
        public string member_location { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        public DateTime birthday { get; set; }
        public string password { get; set; }
        public string username { get; set; }

        // CW2.User_Profile_Attributes
        public string about_me { get; set; }
        public string units { get; set; }
        public string activity_time_preference { get; set; }
        public string marketing_language { get; set; }

        // Additional property for CW2.User_ActivityID
        public int activity_id { get; set; }
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
                string sqlSelect = "SELECT * FROM CW2.User_Information";//add sql command

                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var datatable = new System.Data.DataTable();
                        datatable.Load(reader);

                        string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(datatable);

                        // Close the connection after retrieving data
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Use parameterized query to avoid SQL injection
                string sqlSelect = "SELECT * FROM CW2.User_Information WHERE user_id = @UserId";

                using (SqlCommand command = new SqlCommand(sqlSelect, connection))
                {
                    // Add parameter for UserId
                    command.Parameters.AddWithValue("@UserId", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var dataTable = new System.Data.DataTable();
                        dataTable.Load(reader);

                        string jsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(dataTable);

                        // Close the connection after retrieving data
                        connection.Close();

                        return Content(jsonResult, "application/json");
                    }
                }
            }
        }

        // POST api/<UserInfoController>
       
        [HttpPost]
        public IActionResult Post([FromBody] UserInformationModel user)
        {
            if (user == null)
            {
                return BadRequest("Invalid data");
            }

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Begin a transaction
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert into CW2.User_Profile_Attributes table
                        string sqlInsertAttributes = "INSERT INTO CW2.User_Profile_Attributes " +
                                                     "(user_attributes_id, about_me, units, activity_time_preference, marketing_language) " +
                                                     "VALUES " +
                                                     "(@UserId, @AboutMe, @Units, @ActivityTimePreference, @MarketingLanguage)";

                        using (SqlCommand commandAttributes = new SqlCommand(sqlInsertAttributes, connection, transaction))
                        {
                            
                            commandAttributes.Parameters.AddWithValue("@AboutMe", user.about_me);
                            commandAttributes.Parameters.AddWithValue("@Units", user.units);
                            commandAttributes.Parameters.AddWithValue("@ActivityTimePreference", user.activity_time_preference);
                            commandAttributes.Parameters.AddWithValue("@MarketingLanguage", user.marketing_language);

                            commandAttributes.ExecuteNonQuery();
                        }

                        // Insert into CW2.User_Information table
                        string sqlInsertUserInfo = "INSERT INTO CW2.User_Information " +
                                                   "(user_id, email, member_location, height, weight, birthday, password, user_attributes_id, username) " +
                                                   "VALUES " +
                                                   "(@UserId, @Email, @MemberLocation, @Height, @Weight, @Birthday, @Password, @UserId, @Username)";

                        using (SqlCommand commandUserInfo = new SqlCommand(sqlInsertUserInfo, connection, transaction))
                        {
                            commandUserInfo.Parameters.AddWithValue("@UserId", user.user_id);
                            commandUserInfo.Parameters.AddWithValue("@Email", user.email);
                            commandUserInfo.Parameters.AddWithValue("@MemberLocation", user.member_location);
                            commandUserInfo.Parameters.AddWithValue("@Height", user.height);
                            commandUserInfo.Parameters.AddWithValue("@Weight", user.weight);
                            commandUserInfo.Parameters.AddWithValue("@Birthday", user.birthday);
                            commandUserInfo.Parameters.AddWithValue("@Password", user.password);
                            commandUserInfo.Parameters.AddWithValue("@Username", user.username);

                            commandUserInfo.ExecuteNonQuery();
                        }

                        // Insert into CW2.User_ActivityID table
                        string sqlInsertUserActivity = "INSERT INTO CW2.User_ActivityID (user_id, activity_id) " +
                                                       "VALUES (@UserId, @ActivityId)";

                        using (SqlCommand commandUserActivity = new SqlCommand(sqlInsertUserActivity, connection, transaction))
                        {
                            commandUserActivity.Parameters.AddWithValue("@UserId", user.user_id);
                            commandUserActivity.Parameters.AddWithValue("@ActivityId", user.activity_id);

                            commandUserActivity.ExecuteNonQuery();
                        }

                        // Commit the transaction if everything is successful
                        transaction.Commit();

                        return Ok("User added successfully");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an exception
                        transaction.Rollback();

                        // Log the exception or handle it as needed
                        Console.WriteLine(ex.Message);

                        return BadRequest("Failed to add user");
                    }
                }
            }
        }


        // PUT api/<UserInfoController>/5
        [HttpPut("{userId}")]
        public IActionResult Put(int userId, [FromBody] UserInformationUpdateModel updateModel)
        {
            if (updateModel == null) //checks to see if there is data
            {
                return BadRequest("Invalid update data");
            }

            string connectionString = Configuration.GetConnectionString("DefaultConnection");

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

                        // Update the specified column in CW2.User_Information 
                        string updateColumnQuery = $"UPDATE CW2.User_Information SET {updateModel.ColumnName} = @{updateModel.ColumnName} WHERE user_id = @UserId";
                        using (SqlCommand updateColumnCommand = new SqlCommand(updateColumnQuery, connection, transaction))
                        {
                            updateColumnCommand.Parameters.AddWithValue("@UserId", userId);
                            updateColumnCommand.Parameters.AddWithValue($"@{updateModel.ColumnName}", updateModel.GetValue());

                            updateColumnCommand.ExecuteNonQuery();
                        }

                        // Commit the transaction if everything is successful
                        transaction.Commit();

                        return Ok("User information updated successfully for user_id {userId}");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an exception
                        transaction.Rollback();

                        // return message
                        Console.WriteLine(ex.Message);

                        return BadRequest("Failed to update user information for user_id {userId}");
                    }
                }
            }
        }


        // DELETE api/<UserInfoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
