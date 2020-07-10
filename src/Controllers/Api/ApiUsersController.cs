using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.ApiDevApp;
using Microsoft.AspNetCore.Identity;

namespace BES.Controllers.ApiDevApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

       
        public ApiUsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: api/ApiUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiUser>>> GetApiUser()
        {
            return await _context.ApiUser.ToListAsync();
        }

        // GET: api/ApiUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiUser>> GetApiUser(string id)
        {
            var apiUser = await _context.ApiUser.FindAsync(id);

            if (apiUser == null)
            {
                return NotFound();
            }

            return apiUser;
        }

        // PUT: api/ApiUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApiUser(string id, ApiUser apiUser)
        {
            if (id != apiUser.Email)
            {
                return BadRequest();
            }

            _context.Entry(apiUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ApiUsers
        [HttpPost]
        public async Task<ActionResult<ApiUser>> PostApiUser(ApiUser apiUser)
        {
            bool status = true;
            string message = "Success";            
            //string passwordhash= 
            //var user = _context.Users.Where(a => a.Email == apiUser.Email && a.PhoneNumber==apiUser.Password).FirstOrDefaultAsync();
           var user = await _userManager.FindByEmailAsync(apiUser.Email);
            if (user.PhoneNumber!=apiUser.Password)
            {
                status = false;
                message = "Credential not matched";
                            }
            else
            {
                message = user.RegionalAccess;
            }

            return Ok(new { status, message });
            //_context.ApiUser.Add(apiUser);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetApiUser", new { id = apiUser.Email }, apiUser);
        }
        public async Task<string> GetCurrentUserRegion()
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            //if (usr.RegionalAccess != null)
            return (usr.RegionalAccess);
            //else
            //    return ("a");
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // DELETE: api/ApiUsers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiUser>> DeleteApiUser(string id)
        {
            var apiUser = await _context.ApiUser.FindAsync(id);
            if (apiUser == null)
            {
                return NotFound();
            }

            _context.ApiUser.Remove(apiUser);
            await _context.SaveChangesAsync();

            return apiUser;
        }

        private bool ApiUserExists(string id)
        {
            return _context.ApiUser.Any(e => e.Email == id);
        }
    }
}
