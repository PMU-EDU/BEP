using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.LMS.Models;
using BES.Data;
using Microsoft.AspNetCore.Http;

namespace BES.Areas.LMS.Controllers
{
    [Area("LMS")]
    public class EmpLeaveSummariesController : BaseController
    {        

        public EmpLeaveSummariesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        { }        

        // GET: LMS/EmpLeaveSummaries
        public async Task<IActionResult> Index()
        {
            ViewBag.Inbox = Inbox;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;
            var applicationDbContext = _context.Employee.Include(e => e.Section);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: LMS/EmpLeaveSummaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empLeaveSummary = await _context.EmpLeaveSummary
                .Include(e => e.Employee)
                .Include(e => e.LeaveType)
                .FirstOrDefaultAsync(m => m.EmpLeaveSummaryID == id);
            if (empLeaveSummary == null)
            {
                return NotFound();
            }

            return View(empLeaveSummary);
        }

        // GET: LMS/EmpLeaveSummaries/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmployeeID");
            ViewData["LeaveTypeID"] = new SelectList(_context.Set<LeaveType>(), "LeaveTypeID", "LeaveTypeID");
            return View();
        }

        // POST: LMS/EmpLeaveSummaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmpLeaveSummaryID,Total,Availed,Pending,EmployeeID,LeaveTypeID")] EmpLeaveSummary empLeaveSummary)
        {
            if (ModelState.IsValid)
            {
                empLeaveSummary.Pending = 0;
                _context.Add(empLeaveSummary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmployeeID", empLeaveSummary.EmployeeID);
            ViewData["LeaveTypeID"] = new SelectList(_context.Set<LeaveType>(), "LeaveTypeID", "LeaveTypeID", empLeaveSummary.LeaveTypeID);
            return View(empLeaveSummary);
        }

        // GET: LMS/EmpLeaveSummaries/Edit/5
        public async Task<IActionResult> Edit(int id)
        {           

            var empLeaveSummary = await _context.EmpLeaveSummary.Include(a=>a.LeaveType).Include(a=>a.Employee).Where(a=>a.EmployeeID == id).ToListAsync();
            if (empLeaveSummary == null)
            {
                return NotFound();
            }
            ViewBag.Name = _context.Employee.Find(id).Name;
            return View(empLeaveSummary);
        }

        // POST: LMS/EmpLeaveSummaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, List<EmpLeaveSummary> empLeaveSummaries)
        {            
            if (ModelState.IsValid)
            {
                foreach (var empLeaveSummary in empLeaveSummaries)
                {
                    try
                    {
                        empLeaveSummary.LeaveType = null;
                        _context.Update(empLeaveSummary);
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!EmpLeaveSummaryExists(empLeaveSummary.EmpLeaveSummaryID))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }                    
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }       
            return View(empLeaveSummaries);
        }

        // GET: LMS/EmpLeaveSummaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empLeaveSummary = await _context.EmpLeaveSummary
                .Include(e => e.Employee)
                .Include(e => e.LeaveType)
                .FirstOrDefaultAsync(m => m.EmpLeaveSummaryID == id);
            if (empLeaveSummary == null)
            {
                return NotFound();
            }

            return View(empLeaveSummary);
        }

        // POST: LMS/EmpLeaveSummaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empLeaveSummary = await _context.EmpLeaveSummary.FindAsync(id);
            _context.EmpLeaveSummary.Remove(empLeaveSummary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmpLeaveSummaryExists(int id)
        {
            return _context.EmpLeaveSummary.Any(e => e.EmpLeaveSummaryID == id);
        }
    }
}
