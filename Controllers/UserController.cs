using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AttendanceAPI3.Models;
using AttendanceAPI3.Models.DTOs;


namespace AttendanceAPI3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AttendanceContext _context;

        public UsersController(AttendanceContext context)
        {
            _context = context;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromForm]UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string input = userDto.UserPassword;
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                UserPassword = pass_hash.Hashpassword(input), 
                Age = userDto.Age,
                Gender = userDto.Gender,
                ConfirmPassword=userDto.ConfirmPassword,
                UserRole = userDto.UserRole
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Registeration is Successfull" });    
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null)
                return Unauthorized("Invalid username or email.");

            var result = _context.Users.Where(a => a.UserPassword.Equals(pass_hash.Hashpassword(loginDto.UserPassword))).FirstOrDefault(); ;

            if (result == null)
                return Unauthorized("Invalid password.");

            var sessionId = HttpContext.Session.Id;
            HttpContext.Session.SetString("Email", loginDto.Email);
            Response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            return Ok(new { UserId = user.UserId, Username = user.Username });
        }

        // POST: api/Users/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("SessionId");
            return Ok(new { message = "Successfully logged out." });
        }
    }
}
