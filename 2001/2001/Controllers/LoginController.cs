using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace _2001.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        //gets the entered username and password
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
       

        //sets connection
        public IConfiguration Configuration { get; }

        public LoginController(IConfiguration configuration)
        {
            Configuration = configuration;
        }



        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            

            //checks if username and password has been filled in
            if (@username == null || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Invalid login request");
            }
                string connectionString = Configuration.GetConnectionString("DefaultConnection"); //connects

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open(); //opens connection

                    string query = "SELECT user_id FROM CW2.User_Information WHERE username = @Username AND password = @Password"; //sql command to check if username and password matches a user_id

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username); //adds username and password to variables
                        command.Parameters.AddWithValue("@Password", password); 

                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                         UserLogin.Loggedin = true;
                         return Ok("Login successful");
                        }
                        else
                        {
                            return Unauthorized("Invalid credentials");
                        }
                    }
                }

        }
    }


}
