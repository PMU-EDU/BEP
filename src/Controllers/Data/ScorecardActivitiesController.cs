using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Data;
using BES.Models.Data;
using System.Net;
using System.Net.Mail;

namespace BES.Controllers.Data
{
    public class ScorecardActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ScorecardActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ScorecardActivities
        public async Task<IActionResult> Index(short id)
        {
            var activityList = await _context.ScorecardActivity.Include(a => a.Section).Include(a => a.Component).OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).Where(a => a.ParentID != 0).ToListAsync();
            ViewBag.Total = _context.ScorecardActivity.Count(a => a.Status > 0);
            if (id == 1)
            {
                activityList = activityList.OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).Where(a => a.Status <= id).ToList();
            }
            else if(id == 2)
            {
                activityList = activityList.OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).Where(a => a.Status <= id && a.Status != 1).ToList();
            }
            else if (id == 3)
            {
                activityList = activityList.OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).Where(a => a.Status <= id && a.Status != 1 && a.Status != 2).ToList();
            }
            else if (id == 4)
            {
                activityList = activityList.OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).Where(a => a.IsHead == true || a.Status == 4).ToList();
            }
            ViewBag.OnTrack = activityList.Count(a => a.Status == 1);
            ViewBag.Behind = activityList.Count(a => a.Status == 2);
            ViewBag.Delay = activityList.Count(a => a.Status == 3);
            ViewBag.Completed = activityList.Count(a => a.Status == 4);
            string array = activityList.Select(a => a.meetingStatusList).FirstOrDefault().ToString();
            List<string> names = array.Split(',').Reverse().ToList();
            ViewBag.Columns = names.Count();
            ViewBag.MeetingDates = _context.ScoreCardMeetingSchedule.ToList();
            return View(activityList);
        }
        public async Task<IActionResult> getScorecard(short id)
        {
            var activityList = await _context.ScorecardActivity.Include(a => a.Section).Include(a => a.Component).OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).Where(a => a.ParentID != 0).ToListAsync();
            if(id != 0)
            {
                activityList = activityList.Where(a => a.Status == id).OrderBy(a => a.ComponentID).OrderBy(a => a.SerialNo).ToList();
            }
            ViewBag.Total = activityList.Count(a => a.Status > 0);
            ViewBag.OnTrack = activityList.Count(a => a.Status == 1);
            ViewBag.Behind = activityList.Count(a => a.Status == 2);
            ViewBag.Delay = activityList.Count(a => a.Status == 3);
            ViewBag.Completed = activityList.Count(a => a.Status == 4);
            return PartialView(activityList);
        }
        public async Task<IActionResult> Index2()
        {
            var result = await _context.ScorecardActivity.Include(a => a.Component).Where(a => a.ComponentID == 3).ToListAsync();
            var query = (from p in _context.ScorecardActivity.Include(a=>a.Component).Include(a=>a.Section).Where(a=>a.IsHead == false)
                        join q in _context.ScorecardActivity on p.ParentID equals q.ScorecardActivityID
                        select new ScorecardActivity { ActivityName = p.ActivityName,Comments = p.Section.Name , SerialNo = p.SerialNo, ScorecardActivityID = p.ScorecardActivityID, Target = p.Component.ComponentCode, Achived = p.SerialNo.Replace(".0","") + " " + q.ActivityName }).OrderBy(a=>a.SerialNo);
            return PartialView(query);
        }
        // GET: ScorecardActivities/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scorecardActivity = await _context.ScorecardActivity
                .FirstOrDefaultAsync(m => m.ScorecardActivityID == id);
            if (scorecardActivity == null)
            {
                return NotFound();
            }

            return View(scorecardActivity);
        }       
        public JsonResult GetComponentHead(short componentID)
        {                        
            var activityList = _context.ScorecardActivity.Where(a => a.ComponentID == componentID && a.IsHead == true).Select(s => new { s.ScorecardActivityID, ActivityName = s.SerialNo + " " + s.ActivityName }).ToList();            
            return Json(new SelectList(activityList, "ScorecardActivityID", "ActivityName"));
        }
        private IEnumerable<SelectListItem> GetParentNode()
        {
             var Output = _context.ScorecardActivity.Where(a=>a.IsHead == true && a.ComponentID == 3)
            .Select(c => new SelectListItem() { Text = c.SerialNo + " " + c.ActivityName, Value = c.ScorecardActivityID.ToString() })
            .ToList();
            //var first = new SelectListItem() { Text = "Not Parent", Value = string.Empty };
            //Output.Insert(0, first);
            return Output;
        }
        // GET: ScorecardActivities/Create
        public IActionResult Create()
        {
            ScorecardActivity scorecard = new ScorecardActivity();            
            scorecard.ParentNode = this.GetParentNode();
            scorecard.StartDate = DateTime.Now.Date;
            scorecard.EndDate = DateTime.Now.Date;            
            scorecard.IsHead = true;            
            scorecard.Status = 1;            
            scorecard.SectionID = 1;
            scorecard.IsUpdated = true;
            ViewData["SectionID"] = new SelectList(_context.Section.Where(a => a.SectionID < 10), "SectionID", "Name");
            ViewData["ComponentID"] = new SelectList(_context.Component.Where(a=>a.ComponentID > 2).Select(a=> new { ComponentName = a.ComponentCode + " - " + a.ComponentName,a.ComponentID }), "ComponentID", "ComponentName");
            ViewData["Colors"] = new List<SelectListItem>
                {
                    new SelectListItem {Text = "ON-TRACK", Value = "1"},
                    new SelectListItem {Text = "FALLING BEHIND", Value = "2"},
                    new SelectListItem {Text = "DELAY ALERT", Value = "3"},
                    new SelectListItem {Text = "COMPLETE", Value = "4"}
                };
            int ANo = _context.ScorecardActivity.Count();
            scorecard.ActivityNo = (short)++ANo;
            return View(scorecard);
        }

        // POST: ScorecardActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScorecardActivityID,ActivityNo,SerialNo,ComponentID,ActivityName,Target,Achived,Responsibility,SectionID,StartDate,EndDate,Comments,BaselineStartDate,BaselineEndDate,Duration,Progress,Predecessor,ParentID,IsHead,Status,IsUpdated,meetingStatusList")] ScorecardActivity scorecardActivity)
        {
            if (ModelState.IsValid)
            {
                scorecardActivity.BaselineStartDate = scorecardActivity.StartDate;
                scorecardActivity.BaselineEndDate = scorecardActivity.EndDate;
                if(scorecardActivity.IsHead == false)
                {
                    scorecardActivity.SerialNo = _context.ScorecardActivity.Where(a => a.ScorecardActivityID == scorecardActivity.ParentID).Select(a=>a.SerialNo).FirstOrDefault().ToString() + ".0";                     
                }
                _context.Add(scorecardActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create));
            }
            ViewData["SectionID"] = new SelectList(_context.Section.Where(a => a.SectionID < 10), "SectionID", "Name",scorecardActivity.SectionID);
            ViewData["ComponentID"] = new SelectList(_context.Component.Where(a => a.ComponentID > 2).Select(a => new { ComponentName = a.ComponentCode + " - " + a.ComponentName, a.ComponentID }), "ComponentID", "ComponentName");
            ViewData["Colors"] = new List<SelectListItem>
                {
                    new SelectListItem {Text = "ON-TRACK", Value = "1"},
                    new SelectListItem {Text = "FALLING BEHIND", Value = "2"},
                    new SelectListItem {Text = "DELAY ALERT", Value = "3"},
                    new SelectListItem {Text = "COMPLETE", Value = "4"}
                };
            return View(scorecardActivity);
        }
        public IActionResult SEmail(short? id)
        {
            SendEmail("EU-Scorecard Updates", "Test SMS");
            return View();
        }
        // GET: ScorecardActivities/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            var scorecardActivity = await _context.ScorecardActivity.FindAsync(id);
            if (scorecardActivity == null)
            {
                return NotFound();
            }
            ViewData["SectionID"] = new SelectList(_context.Section.Where(a => a.SectionID < 10), "SectionID", "Name", scorecardActivity.SectionID);
            ViewData["ComponentID"] = new SelectList(_context.Component.Where(a => a.ComponentID > 2).Select(a => new { ComponentName = a.ComponentCode + " - " + a.ComponentName, a.ComponentID }), "ComponentID", "ComponentName",scorecardActivity.ComponentID);
            ViewData["Colors"] = new List<SelectListItem>
                {
                    new SelectListItem {Text = "ON-TRACK", Value = "1"},
                    new SelectListItem {Text = "FALLING BEHIND", Value = "2"},
                    new SelectListItem {Text = "DELAY ALERT", Value = "3"},
                    new SelectListItem {Text = "COMPLETE", Value = "4"}
                };
            ViewBag.IsUpdated = scorecardActivity.IsUpdated;
            return View(scorecardActivity);
        }

        // POST: ScorecardActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("ScorecardActivityID,ActivityNo,SerialNo,ComponentID,ActivityName,Target,Achived,Responsibility,SectionID,StartDate,EndDate,Comments,BaselineStartDate,BaselineEndDate,Duration,Progress,Predecessor,ParentID,IsHead,Status,IsUpdated,meetingStatusList")] ScorecardActivity scorecardActivity)
        {
            if (id != scorecardActivity.ScorecardActivityID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    scorecardActivity.meetingStatusList = scorecardActivity.meetingStatusList + "," + scorecardActivity.Status.ToString();
                    scorecardActivity.IsUpdated = true;
                    scorecardActivity.meetingStatusList.Replace("5", scorecardActivity.Status.ToString());
                    _context.Update(scorecardActivity);                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScorecardActivityExists(scorecardActivity.ScorecardActivityID))
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
            ViewData["SectionID"] = new SelectList(_context.Section.Where(a => a.SectionID < 10), "SectionID", "Name", scorecardActivity.SectionID);
            ViewData["ComponentID"] = new SelectList(_context.Component.Where(a => a.ComponentID > 2).Select(a => new { ComponentName = a.ComponentCode + " - " + a.ComponentName, a.ComponentID }), "ComponentID", "ComponentName", scorecardActivity.ComponentID);
            ViewData["Colors"] = new List<SelectListItem>
                {
                    new SelectListItem {Text = "ON-TRACK", Value = "1"},
                    new SelectListItem {Text = "FALLING BEHIND", Value = "2"},
                    new SelectListItem {Text = "DELAY ALERT", Value = "3"},
                    new SelectListItem {Text = "COMPLETE", Value = "4"}
                };
            return View(scorecardActivity);
        }

        // GET: ScorecardActivities/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scorecardActivity = await _context.ScorecardActivity
                .FirstOrDefaultAsync(m => m.ScorecardActivityID == id);
            if (scorecardActivity == null)
            {
                return NotFound();
            }

            return View(scorecardActivity);
        }

        // POST: ScorecardActivities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var scorecardActivity = await _context.ScorecardActivity.FindAsync(id);
            _context.ScorecardActivity.Remove(scorecardActivity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScorecardActivityExists(short id)
        {
            return _context.ScorecardActivity.Any(e => e.ScorecardActivityID == id);
        }
        //SendEmail(complaint.Subject, "<b> Name: </b>" + complaint.Name+ "<br><b> CNIC: </b>" + complaint.CNIC+ "<br><b> Contact# </b>" + complaint.MobileNo + "<br><br><br>" + complaint.Description);
        private bool SendEmail(string sub, string text)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress("no.reply.sepmu@gmail.com", "To PMU"));
                message.From = new MailAddress("no.reply.sepmu@gmail.com", "From PMU");
                //message.CC.Add(new MailAddress("cc@email.com", "CC Name"));
                //message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));
                message.Subject = sub;
                message.Body = text;
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("no.reply.sepmu@gmail.com", "Gpeb##1234");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
            return true;
        }
    }
}
