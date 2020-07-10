using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;

namespace BES.Controllers.Data
{
    public class ScoreCardMeetingSchedulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScoreCardMeetingSchedulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ScoreCardMeetingSchedules
        public async Task<IActionResult> Index()
        {
            return View(await _context.ScoreCardMeetingSchedule.ToListAsync());
        }

        // GET: ScoreCardMeetingSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreCardMeetingSchedule = await _context.ScoreCardMeetingSchedule
                .FirstOrDefaultAsync(m => m.ScoreCardMeetingScheduleID == id);
            if (scoreCardMeetingSchedule == null)
            {
                return NotFound();
            }

            return View(scoreCardMeetingSchedule);
        }

        // GET: ScoreCardMeetingSchedules/Create
        public IActionResult Create()
        {
            ScoreCardMeetingSchedule Obj = new ScoreCardMeetingSchedule();
            Obj.Frequency = 1;
            Obj.ReminderSofar = 1;
            Obj.CurrentDate = DateTime.Now.Date;
            Obj.LastMeetingDate = DateTime.Now.Date;
            Obj.DateOfReminderStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Monday);
            return View(Obj);
        }

        // POST: ScoreCardMeetingSchedules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScoreCardMeetingScheduleID,LastMeetingDate,DateOfReminderStart,Frequency,ReminderSofar,CurrentDate")] ScoreCardMeetingSchedule scoreCardMeetingSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scoreCardMeetingSchedule);
                var objList = await _context.ScorecardActivity.ToListAsync();
                foreach(var obj in objList)
                {
                    if(obj.Status == 4)
                    {
                        obj.meetingStatusList = obj.meetingStatusList + ",4";
                        obj.IsUpdated = true;
                    }
                    else if(obj.Status == 0)
                    {
                        obj.meetingStatusList = obj.meetingStatusList + ",0";
                        obj.IsUpdated = true;
                    }
                    else
                    {
                        obj.meetingStatusList = obj.meetingStatusList + ",5";
                        obj.IsUpdated = false;
                    }
                    _context.Update(obj);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(scoreCardMeetingSchedule);
        }

        // GET: ScoreCardMeetingSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreCardMeetingSchedule = await _context.ScoreCardMeetingSchedule.FindAsync(id);
            if (scoreCardMeetingSchedule == null)
            {
                return NotFound();
            }
            return View(scoreCardMeetingSchedule);
        }

        // POST: ScoreCardMeetingSchedules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScoreCardMeetingScheduleID,LastMeetingDate,DateOfReminderStart,Frequency,ReminderSofar,CurrentDate")] ScoreCardMeetingSchedule scoreCardMeetingSchedule)
        {
            if (id != scoreCardMeetingSchedule.ScoreCardMeetingScheduleID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scoreCardMeetingSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScoreCardMeetingScheduleExists(scoreCardMeetingSchedule.ScoreCardMeetingScheduleID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(scoreCardMeetingSchedule);
        }

        // GET: ScoreCardMeetingSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scoreCardMeetingSchedule = await _context.ScoreCardMeetingSchedule
                .FirstOrDefaultAsync(m => m.ScoreCardMeetingScheduleID == id);
            if (scoreCardMeetingSchedule == null)
            {
                return NotFound();
            }

            return View(scoreCardMeetingSchedule);
        }

        // POST: ScoreCardMeetingSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scoreCardMeetingSchedule = await _context.ScoreCardMeetingSchedule.FindAsync(id);
            _context.ScoreCardMeetingSchedule.Remove(scoreCardMeetingSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScoreCardMeetingScheduleExists(int id)
        {
            return _context.ScoreCardMeetingSchedule.Any(e => e.ScoreCardMeetingScheduleID == id);
        }
    }
}
