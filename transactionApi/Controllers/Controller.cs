using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using transactionApi.Models;

namespace transactionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Controller : ControllerBase
    {
        private readonly Context _context;
        private readonly UserServices _services;

        public Controller(Context context, UserServices services)
        {
            _context = context;
            _services = services;

        }


        [HttpPost]
        [Route("/makeTransaction")]
        public IActionResult makeTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest(new {message = "Greska!" });
            }

            User sender = _context.Users.FirstOrDefault(u => u.account == transaction.senderAccount);
            User receiver = _context.Users.FirstOrDefault(u => u.account == transaction.receiverAccount);
            int amount = transaction.amount;

            if (sender == null || receiver == null)
            {
                transaction.TransactionStatus = 0;
                transaction.transactionDescription = "Korisnik ne postoji";
                _context.Transactions.Add(transaction); // upisujemo transakciju u bazu
                _context.SaveChanges(); // cuvamo promene u bazi
                return BadRequest(new
                {
                    message = "Korisnik ne postoji!"
                });

            }

            if(sender.balance < amount)
            {
                transaction.TransactionStatus = 0;
                transaction.transactionDescription = "Nedovoljno sredstava";
                _context.Transactions.Add(transaction); // upisujemo transakciju u bazu
                _context.SaveChanges(); // cuvamo promene u bazi
                return BadRequest(new
                {
                    message = "Nemate dovoljno sredstava na racunu!"
                });
            }
            if(amount <= 0)
            {
                transaction.transactionDescription = "Iznos manji od 0";
                transaction.TransactionStatus = 0;
                _context.Transactions.Add(transaction); // upisujemo transakciju u bazu
                _context.SaveChanges(); // cuvamo promene u bazi
                return BadRequest(new
                {
                    message = "Iznos ne moze biti manji od 0!"
                });
            }

            sender.balance -= amount;
            receiver.balance += amount;
            transaction.transactionType = "";
            transaction.TransactionStatus = 1;
            transaction.transactionDescription = "Uspesna transakcija";
            _context.Transactions.Add(transaction); // upisujemo transakciju u bazu
            _context.SaveChanges(); // cuvamo promene u bazi
            return Ok(new
                {
                    message =  "Uspesno ste izvrsili transakciju!"
                });
        }

        [Authorize]
        [HttpGet]
        [Route("/checkBalance")]
        public async Task<IActionResult> checkBalance(int userID)
        {
            try
            {
                //User user = new User();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userID == userID); // proveravamo da li postoji korisnik

                if (user == null) { return BadRequest("Korisnik ne postoji"); }

                var result = new
                {
                    UserName = user.userName,
                    Balance = user.balance
                };
                return Ok(result);
            }
           

            catch (Exception ex)
{
                // Log the exception
                Console.WriteLine($"An error occurred in checkBalance: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Route("/getTransactions")]
        public async Task<IActionResult> getTransactions(string userID)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.account == userID);
            if (user == null)
            {
                return BadRequest("Korisnik ne postoji");
            }
            var transactions = await _context.Transactions
        .Where(t => (t.senderAccount == userID || t.receiverAccount == userID))
                .ToListAsync();

            var transactionDetails = transactions.Select(t => new
            {
                TransactionID = t.TransactionId,
                transactionName = t.TransactionName,
                sender = t.senderAccount,
                Amount = t.amount,
                dateTime = t.DateTime,
                transactionStatus = t.TransactionStatus, // Include transactionStatus here
                transactionType = (t.senderAccount == userID) ? "odliv" : "priliv"
            }).ToArray();

            var result = new
            {
                UserID = user.userID,
                UserName = user.userName,
                Transactions = transactionDetails
            };

            // Return the result
            return Ok(result);
        }
        private string GetUserName(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.userID == userId);
            return user?.userName ?? "Unknown";
        }

        [HttpPost]
        [Route("/createUser")]
        public async Task<IActionResult> createUser([FromBody] User user)
        {

            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            user.userRole = "user";
            _context.Add(user);
            await _context.SaveChangesAsync();

            var message = "Korisnik uspesno kreiran.";
            var statusCode = 200;

            return new ContentResult
            {
                Content = message,
                StatusCode = statusCode,
                ContentType = "text/html"
            };
        }

        [Authorize("RequireLoggedIn")]
        [HttpGet]
        [Route("/getUserData")]
        public async Task<IActionResult> GetUserData()
        {
            try
            {
                // Retrieve user information based on the authenticated user (token)
                var userNameClaim = HttpContext.User.FindFirst(ClaimTypes.Name);

                if (userNameClaim == null)
                {
                    return BadRequest("Unable to retrieve user information");
                }

             

                string userName = userNameClaim.Value;

                // Retrieve additional user data from the database or any other source
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userName == userName); // proveravamo da li postoji korisnik

                if (user == null) { return BadRequest("Korisnik ne postoji"); }

                var transactions = await _context.Transactions
        .Where(t => (t.senderAccount == user.account || t.receiverAccount == user.account) && t.TransactionStatus == 1)
                .ToListAsync();

                var transactionDetails = transactions.Select(t => new
                {
                    TransactionID = t.TransactionId,
                    transactionName = t.TransactionName,
                    sender = t.senderAccount,
                    receiver = t.receiverAccount,
                    Amount = t.amount,
                    dateTime = t.DateTime,
                    transactionStatus = t.TransactionStatus, // Include transactionStatus here
                    transactionType = (t.senderAccount == user.account) ? "odliv" : "priliv",
                    UserName = (t.senderAccount == user.account) ? _context.Users.FirstOrDefault(u => u.account == t.receiverAccount)?.userName : _context.Users.FirstOrDefault(u => u.account == t.senderAccount)?.userName
                }).ToArray();

                var result = new
                {
                    UserName = user.userName,
                    Balance = user.balance,
                    Account = user.account,
                    Transactions = transactionDetails
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"Exception during GetUserData: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login([FromBody] User loginRequest)
        {
            try
            {
                // Validate the incoming request
                if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.userName) || string.IsNullOrWhiteSpace(loginRequest.password))
                {
                    return BadRequest("Invalid login request");
                }

                // Verify password
                bool isPasswordCorrect = _services.VerifyPassword(loginRequest.userName, loginRequest.password);
                if (!isPasswordCorrect)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                // Generate and save token
                var token = _services.GenerateJwtToken(loginRequest.userName);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userName == loginRequest.userName);
                if (user != null)
                {
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Successful login", userId = user.userID, token });
                }
                else
                {
                    // Handle the case where the user is not found
                    return NotFound(new { message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly
                Console.WriteLine($"Exception during login: {ex.Message}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }



    }
}
