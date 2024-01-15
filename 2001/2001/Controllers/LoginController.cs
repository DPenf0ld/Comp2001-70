using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Text;

namespace _2001.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private const string ApiBaseUrl = "https://web.socem.plymouth.ac.uk/COMP2001/auth/api/users";
        public IConfiguration Configuration { get; }

        public LoginController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string Email, string Password)
        {
            if (@Email == null || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password)) //NOT WORKING
            {
                return BadRequest("Invalid login request");
            }
            string connectionString = Configuration.GetConnectionString("DefaultConnection"); //connects

            using (HttpClient client = new HttpClient())
            {
                var UserDetails = new
                {
                    Email = Email,
                    Password = Password
                };

                string jsonData = JsonConvert.SerializeObject(UserDetails);
                HttpResponseMessage response = await client.PostAsync(ApiBaseUrl, new StringContent(jsonData, Encoding.UTF8, "application/json"));

                // print to console
                string responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseContent);

                // Parse to array (can get second element which is either true or false)
                var parsedResponse = JsonConvert.DeserializeObject<string[]>(responseContent);

                // Changes the result to just get second element (true or false)
                if (parsedResponse != null && parsedResponse.Length >= 2 && parsedResponse[1].Equals("True", StringComparison.OrdinalIgnoreCase))
                {

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open(); //opens connection

                        string query = "SELECT user_id FROM CW2.User_Information WHERE email = @Email AND password = @Password"; //sql command to check if email and password matches a user_id

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Email", Email); //adds email and password to variables
                            command.Parameters.AddWithValue("@Password", Password);

                            var result = command.ExecuteScalar();

                            if (result != null && int.TryParse(result.ToString(), out int userId))
                            {
                                UserLogin.userloginid = userId;
                                UserLogin.Loggedin = true;
                                UserLogin.admin = true;
                                return Ok("Logged in as admin successfully");
                            }
                            else
                            {
                                return Unauthorized("Invalid credentials");
                            }
                        }
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open(); //opens connection

                        string query = "SELECT user_id FROM CW2.User_Information WHERE email = @Email AND password = @Password"; //sql command to check if email and password matches a user_id

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Email", Email); //adds email and password to variables
                            command.Parameters.AddWithValue("@Password", Password);

                            var result = command.ExecuteScalar();

                            if (result != null && int.TryParse(result.ToString(), out int userId))
                            {
                                UserLogin.userloginid = userId;
                                UserLogin.Loggedin = true;
                                return Ok("Logged in as user successfully");
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
    }
}




