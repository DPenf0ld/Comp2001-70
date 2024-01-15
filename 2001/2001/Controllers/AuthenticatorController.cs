using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace _2001.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatorController : ControllerBase
    {
        private const string ApiBaseUrl = "https://web.socem.plymouth.ac.uk/COMP2001/auth/api/users";

        [HttpPost]
        public async Task<IActionResult> Post(string Email, string Password)
        {
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

                // Parse to array
                var parsedResponse = JsonConvert.DeserializeObject<string[]>(responseContent);

                // Changes the result to just get second element (true or false)
                if (parsedResponse != null && parsedResponse.Length >= 2 && parsedResponse[1].Equals("True", StringComparison.OrdinalIgnoreCase))
                {
                    return Ok("Logged in as admin");
                }
                else
                {
                    return Ok("Logged in as user");
                }
            }
        }
    }
}




