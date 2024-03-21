using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using transactionApi;

namespace transactionApi.Models
{

    public class UserServices : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _jwtSecret;

        private readonly SymmetricSecurityKey _securityKey;
        private readonly Context _context;

        public UserServices(Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _jwtSecret = configuration.GetValue<string>("Jwt:SecretKey");
        }

        public string GetJwtSecretKey()
        {
            return _jwtSecret;
        }

        public User GetUserByUsernameAndPassword(string username, string password)
        {
            return _context.Users.FirstOrDefault(u => u.userName == username && u.password == password);
        }

        public int GetUserIdByUsername(string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.userName == username);
  

            if (user != null)
            {
                return user.userID;
            }

            // Handle the case where the user is not found (return a default value or throw an exception)
            throw new InvalidOperationException($"User with username '{username}' not found.");
        }

        public string GetUserByAccount(string account)
        {
            var user = _context.Users.FirstOrDefault(u => u.account == account);


            if (user != null)
            {
                return user.userName;
            }

            // Handle the case where the user is not found (return a default value or throw an exception)
            throw new InvalidOperationException($"User with username '{user.userName}' not found.");
        }

        public bool VerifyPassword(string username, string enteredPassword)
        {
            var user = _context.Users.FirstOrDefault(u => u.userName == username);

            if (user != null)
            {
                Console.WriteLine($"Entered Password: {enteredPassword}");
                Console.WriteLine($"Stored Hashed Password: {user.password}");
                return BCrypt.Net.BCrypt.Verify(enteredPassword, user.password);
            }

            return false; // User not found
        }

        public string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "rilak",
                audience: "rilak",
                 claims: new[]
                    {
                        new Claim(ClaimTypes.Name, username),
            // Add other claims as needed
                },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IActionResult> getUserData(string userName)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userName == userName);

                if (user == null)
                {
                    return NotFound("Korisnik ne postoji"); // Return 404 Not Found
                }

                return Ok(user); // Return 200 OK with the user data
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"Exception during GetUserData: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }




    }
}
