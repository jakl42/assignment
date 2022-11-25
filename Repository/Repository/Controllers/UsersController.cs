using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Models;
using System.Web.Helpers;
using System.Net.Http;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Azure.Core;

namespace Repository.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUsers()
        {
            _logger.LogInformation("Log message for GET: api/Users");

            if (_context.Users == null)
            {
                return NotFound();
            }

            return await _context.Users.Select(x => UserToGetDTO(x)).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserGetDTO>> GetUser(long id)
        {
            _logger.LogInformation("Log message for GET: api/Users/id");

            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToGetDTO(user);
        }
        
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(long id, User user)
        {
            _logger.LogInformation("Log message for PUT: api/Users/id");

            if (id != user.Id)
            {
                return BadRequest();
            }

            user.Password = Crypto.HashPassword(user.Password);
            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            _logger.LogInformation("Updated user {@User}", user);

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserPostDTO>> PostUser(UserPostDTO userDTO)
        {
            _logger.LogInformation("Log message for POST: api/Users");

            if (_context.Users == null)
            {
                return Problem("Entity set 'UserContext.Users'  is null.");
            }

            var user = new User
            {
                UserName = userDTO.UserName,
                FullName = userDTO.FullName,
                Email = userDTO.Email,
                MobileNumber = userDTO.MobileNumber,
                Language = userDTO.Language,
                Culture = userDTO.Culture,
                Password = Crypto.HashPassword(userDTO.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Added user {@User}", user);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, UserToGetDTO(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            _logger.LogInformation("Log message for DELETE: api/Users/id");

            if (_context.Users == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted user {@User}", user);

            return NoContent();
        }

        // POST: api/Users/5/validate
        [HttpPost("{id}/validate")]
        public async Task<ActionResult<UserValidateDTO>> PostUserPassword(long id, UserValidateDTO userDTO)
        {
            _logger.LogInformation("Log message for api/Users/id/validate");

            if (id != userDTO.Id)
            {
                return BadRequest();
            }

            var user = await _context.Users.FindAsync(id);

            if (userDTO.UserName != user.UserName)
            {
                return BadRequest();
            }

            if (Crypto.VerifyHashedPassword(user.Password, userDTO.Password))
            {
                _logger.LogInformation("Validated user's password {@User}", user);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        private bool UserExists(long id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private static UserGetDTO UserToGetDTO(User user) =>
            new UserGetDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                MobileNumber = user.MobileNumber,
                Language = user.Language,
                Culture = user.Culture
            };
    }
}
