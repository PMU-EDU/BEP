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
    public class UCsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UCsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UCs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UC>>> GetUCs()
        {
            bool status = true;
            string message = "Success";
            var ucsList = await _context.UCs.Where(a=>a.UCID > 0).Select(a => new { a.TehsilID, a.UCID, a.UCName }).ToListAsync();
            return Ok(new { status, message, ucsList });
        }

        // GET: api/UCs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UC>> GetUC(int id)
        {
            var uC = await _context.UCs.FindAsync(id);

            if (uC == null)
            {
                return NotFound();
            }

            return uC;
        }

        // PUT: api/UCs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUC(int id, UC uC)
        {
            if (id != uC.UCID)
            {
                return BadRequest();
            }

            _context.Entry(uC).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UCExists(id))
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

        // POST: api/UCs
        [HttpPost]
        public async Task<ActionResult<UC>> PostUC(UC uC)
        {
            _context.UCs.Add(uC);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUC", new { id = uC.UCID }, uC);
        }

        // DELETE: api/UCs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UC>> DeleteUC(int id)
        {
            var uC = await _context.UCs.FindAsync(id);
            if (uC == null)
            {
                return NotFound();
            }

            _context.UCs.Remove(uC);
            await _context.SaveChangesAsync();

            return uC;
        }

        private bool UCExists(int id)
        {
            return _context.UCs.Any(e => e.UCID == id);
        }
    }
}
