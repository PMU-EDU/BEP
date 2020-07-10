using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;

namespace BES.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class TehsilsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TehsilsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tehsils
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tehsil>>> GetTehsils()
        {
            bool status = true;
            string message = "Success";
            var tehsilList = await _context.Tehsils.Select(a => new { a.DistrictID, a.TehsilID, a.TehsilName }).ToListAsync();
            return Ok(new { status, message, tehsilList });
        }

        // GET: api/Tehsils/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tehsil>> GetTehsil(int id)
        {
            var tehsil = await _context.Tehsils.FindAsync(id);

            if (tehsil == null)
            {
                return NotFound();
            }

            return tehsil;
        }

        // PUT: api/Tehsils/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTehsil(int id, Tehsil tehsil)
        {
            if (id != tehsil.TehsilID)
            {
                return BadRequest();
            }

            _context.Entry(tehsil).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TehsilExists(id))
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

        // POST: api/Tehsils
        [HttpPost]
        public async Task<ActionResult<Tehsil>> PostTehsil(Tehsil tehsil)
        {
            _context.Tehsils.Add(tehsil);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTehsil", new { id = tehsil.TehsilID }, tehsil);
        }

        // DELETE: api/Tehsils/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Tehsil>> DeleteTehsil(int id)
        {
            var tehsil = await _context.Tehsils.FindAsync(id);
            if (tehsil == null)
            {
                return NotFound();
            }

            _context.Tehsils.Remove(tehsil);
            await _context.SaveChangesAsync();

            return tehsil;
        }

        private bool TehsilExists(int id)
        {
            return _context.Tehsils.Any(e => e.TehsilID == id);
        }
    }
}
