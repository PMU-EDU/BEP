using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;

namespace BES.Controllers.ApiDevApp
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndicatorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IndicatorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Indicators
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Indicator>>> GetIndicator()
        {
            bool status = true;
            string message = "Success";
            var indicatorList = await _context.Indicator.Where(a => a.IndicatorID >= 29 && a.IndicatorID <= 40 && a.IsEvidenceFromApp == true).Select(a => new {a.IndicatorID, a.IndicatorName, IsFeeder = a.IsFeeder ? 1 : 0, IsPotential = a.IsPotential ? 1 : 0, IsNextLevel = a.IsNextLevel ? 1 : 0, a.bifurcationToApp }).ToListAsync();
            return Ok(new { status, message, indicatorList });
        }

        // GET: api/Indicators/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Indicator>> GetIndicator(int id)
        {
            var indicator = await _context.Indicator.FindAsync(id);

            if (indicator == null)
            {
                return NotFound();
            }

            return indicator;
        }

        // PUT: api/Indicators/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIndicator(int id, Indicator indicator)
        {
            if (id != indicator.IndicatorID)
            {
                return BadRequest();
            }

            _context.Entry(indicator).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IndicatorExists(id))
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

        // POST: api/Indicators
        [HttpPost]
        public async Task<ActionResult<Indicator>> PostIndicator(Indicator indicator)
        {
            _context.Indicator.Add(indicator);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIndicator", new { id = indicator.IndicatorID }, indicator);
        }

        // DELETE: api/Indicators/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Indicator>> DeleteIndicator(int id)
        {
            var indicator = await _context.Indicator.FindAsync(id);
            if (indicator == null)
            {
                return NotFound();
            }

            _context.Indicator.Remove(indicator);
            await _context.SaveChangesAsync();

            return indicator;
        }

        private bool IndicatorExists(int id)
        {
            return _context.Indicator.Any(e => e.IndicatorID == id);
        }
    }
}
