using LoginAndRegisterAPI.Context;
using LoginAndRegisterAPI.Helpers;
using LoginAndRegisterAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace LoginAndRegisterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _authContext;
        public UserController(AppDbContext appDbContext)
        {
            _authContext = appDbContext;
        }

        // User login method
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userObj)
        {
            if(userObj == null)
                return BadRequest();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(x => x.Username == userObj.Username);

            if(user == null)
                return NotFound(new {Message = "User Not Found!"});

            if (!PasswordHasher.VerifyPassword(userObj.Password, user.Password))
            {
                return BadRequest(new { Message = "Password is Incorrect." });
            }

            user.Token = CreateJwtToken(user);

            return Ok(new
            {
                Token = user.Token,
                Message = "Login Success!"
            });

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User userObj)
        {
            if (userObj == null)
                return BadRequest();

            // Check Username

            if(await CheckUserNameExistAsync(userObj.Username))
                return BadRequest(new { Message = "Username Already Exist!" });

            // Check Email 

            if (await CheckEmailExistAsync(userObj.Email))
                return BadRequest(new { Message = "Email Already Exist!" });

            // Check Password Strength

            var pass = CheckPasswordStrength(userObj.Password);

            if (!string.IsNullOrEmpty(pass))
                return BadRequest(new { Message = pass.ToString() });


            userObj.Password = PasswordHasher.HashPassword(userObj.Password);
            userObj.Role = "User";
            userObj.Token = "";
            await _authContext.Users.AddAsync(userObj);
            await _authContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "User Registered Successfully."
            });
        }

        // When user register, Check whether the username is already exist or not.
        private Task<bool> CheckUserNameExistAsync(string username)
            => _authContext.Users.AnyAsync(x => x.Username == username);

        // When user register, Check whether the email is already exist or not.
        private Task<bool> CheckEmailExistAsync(string email) 
            => _authContext.Users.AnyAsync(y => y.Email == email);

        private string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if(password.Length < 8)
                sb.Append("Password should have at least 8 character(s) long." + Environment.NewLine);

            if (!Regex.IsMatch(password, @"^(?=.*[A-Z])"))
            {
                sb.Append("Password Should have at least 1 Capital character(s)." + Environment.NewLine);
            }

            if (!Regex.IsMatch(password, @"^(?=.*[a-z])"))
            {
                sb.Append("Password Should have at least 1 small character(s)" + Environment.NewLine);
            }

            if (!Regex.IsMatch(password, @"^(?=.*[0-9])"))
            {
                sb.Append("Password Should have at least 1 numeric character(s)" + Environment.NewLine);
            }

            if (!Regex.IsMatch(password, @"^(?=.*[!@#$%^&*()_+{}[\]:;<>,.?~\\/-])"))
            {
                sb.Append("Password Should have at least 1 special character(s)" + Environment.NewLine);
            }

            return sb.ToString();

        }

        private string CreateJwtToken(User user)
        {
            /* 
             * Create a new instance of JwtSecurityTokenHandler, which is responsible \
             * handling the creation and validation of JWTs. 
             */
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            /* Converts a secret key (in this case, a string) into a byte array. 
             * This key is used for signing the JWT.
             */
            var key = Encoding.ASCII.GetBytes("veryverysecret.....");

            /* Creates a ClaimsIdentity containing claims representing user information. 
             * In this example, it includes the user's role and full name.
             */
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            });

            // Create signing credentials using a symmetric key and the HMACSHA256 algorithm
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            /* Configures a SecurityTokenDescriptor with the 
             * identity, 
             * expiration time(1 day in this case)(AddDays(1) for days), and 
             * signing credentials.
             */
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddHours(12),
                SigningCredentials = credentials
            };

            // Uses the JwtSecurityTokenHandler to create a JWT token using the provided token descriptor.
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            // Write the JWT token to a string and return it
            return jwtTokenHandler.WriteToken(token);
        }

        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _authContext.Users.ToListAsync());
        }

    }
}
