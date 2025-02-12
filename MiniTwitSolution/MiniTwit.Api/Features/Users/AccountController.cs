using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MiniTwit.Api.Features.Users
{
    public class AccountController : Controller
    {
        private readonly MiniTwitDbContext _db;

        // Inject the DbContext via the constructor
        public AccountController(MiniTwitDbContext db)
        {
            _db = db;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login(LoginRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["FlashMessage"] = "Please fill out the form correctly.";
                return Redirect("/login");
            }

            // Look up the user by username
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (user == null)
            {
                TempData["FlashMessage"] = "Invalid username or password.";
                return Redirect("/login");
            }

            // Check the password.
            // In a production app, use a proper password hashing mechanism.
            // For example, if you're using ASP.NET Core Identity, use its password hasher.
            if (!CheckPassword(model.Password, user.PwHash))
            {
                TempData["FlashMessage"] = "Invalid username or password.";
                return Redirect("/login");
            }

            // On a successful login, store user info in session (or use authentication cookies).
            HttpContext.Session.SetInt32("UserId", user.UserId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.UserId.ToString())
                // add other claims as needed
            };

            var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
            await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

            TempData["FlashMessage"] = "You were logged in";
            return Redirect("/"); // Redirect to the timeline, for example.
        }

        // Dummy password check. Replace with proper password hashing/verification.
        private bool CheckPassword(string password, string storedHash)
        {
            // For demonstration only: compare strings directly.
            // Replace with a secure password verification (e.g., using BCrypt or ASP.NET Core Identity).
            return GeneratePasswordHash(password) == storedHash;
        }

        [HttpGet("/register")]
        public IActionResult Register()
        {
            return View();
        }

        // POST /register - Process the registration form.
        [HttpPost("/register")]
        public async Task<IActionResult> Register(RegisterRequestModel model)
        {
            // Basic validation similar to the Python version:
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                TempData["FlashMessage"] = "You have to enter a username.";
                return Redirect("/register");
            }

            if (string.IsNullOrWhiteSpace(model.Email) || !model.Email.Contains("@"))
            {
                TempData["FlashMessage"] = "You have to enter a valid email address.";
                return Redirect("/register");
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                TempData["FlashMessage"] = "You have to enter a password.";
                return Redirect("/register");
            }

            if (model.Password != model.Password2)
            {
                TempData["FlashMessage"] = "The two passwords do not match.";
                return Redirect("/register");
            }

            // Check if the username is already taken.
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                TempData["FlashMessage"] = "The username is already taken.";
                return Redirect("/register");
            }

            // Create the user. In a real application, use a secure password hasher.
            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                // Generate a hashed password (this example uses a simple MD5 hash for demonstration;
                // please use a secure method like BCrypt or ASP.NET Core Identity in production).
                PwHash = GeneratePasswordHash(model.Password)
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();

            TempData["FlashMessage"] = "You were successfully registered and can now log in.";
            return Redirect("/login");
        }

        // Simple (insecure) MD5 hash generator for demonstration.
        // DO NOT use MD5 in production; use a secure password hashing algorithm.
        private string GeneratePasswordHash(string password)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(password);
                var hashBytes = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        [HttpGet("/logout", Order = 0)]
        public async Task<IActionResult> Logout()
        {
            // Remove the user ID from session.
            HttpContext.Session.Remove("UserId");

            // Sign out of the cookie authentication.
            await HttpContext.SignOutAsync("CookieAuth");

            TempData["FlashMessage"] = "You were logged out.";
            return Redirect("/public");
        }

    }
}