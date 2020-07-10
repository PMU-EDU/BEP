using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.LMS.Models;
using BES.Data;
using Microsoft.AspNetCore.Authorization;
using BES.Areas.LMS.Models.View_Models;
using Microsoft.AspNetCore.Http;

namespace BES.Areas.LMS.Controllers
{
    [Area("LMS")]
    public class EmployeesController : BaseController
    {        

        public EmployeesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {            
        }

        // GET: LMS/Employees
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
        public async Task<IActionResult> Index2()
        {
            var applicationDbContext = _context.Employee.Include(e => e.Section);
            return PartialView(await applicationDbContext.ToListAsync());
        }
        // GET: LMS/Employees
        public ActionResult TreeView()
        {
            ViewBag.Inbox = Inbox;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;
            int totalStaff = 0;
            List<EmployeeTreeView> EmpSectionWise = new List<EmployeeTreeView>();
            var sections = _context.Section.Where(a => a.SectionID < 10).ToList();
            foreach(var section in sections)
            {
                EmployeeTreeView Obj = new EmployeeTreeView();
                Obj.SectionID = section.SectionID;
                Obj.Name = section.Name;
                Obj.EmployeeList = _context.Employee.Where(a => a.SectionID == section.SectionID).ToList();
                EmpSectionWise.Add(Obj);
                totalStaff = totalStaff + Obj.EmployeeList.Count;
            }
            ViewBag.totalStaff = totalStaff;
            return View(EmpSectionWise.ToList());
        }       
        // GET: LMS/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Section)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        public JsonResult GetSectionEmployee(int sectionID)
        {
            List<Employee> employeeList = new List<Employee>();
            employeeList = _context.Employee.Where(a => a.SectionID == sectionID || a.SectionID == 10).ToList();           
            return Json(new SelectList(employeeList, "EmployeeID", "Name"));
        }

        [Authorize(Roles = "HR")]
        // GET: LMS/Employees/Create
        public IActionResult Create()
        {
            ViewBag.Inbox = Inbox;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;
            ViewData["Gender"] = new List<SelectListItem>
            {
                new SelectListItem {Text = "Male", Value = "Male"},
                new SelectListItem {Text = "Female", Value = "Female"}
            };
            ViewData["SupervisorID"] = new SelectList(_context.Employee.Where(a=>a.SectionID == 1 || a.SectionID == 10), "SectionID", "Name");
            ViewData["SectionID"] = new SelectList(_context.Section.Where(a=>a.SectionID < 10), "SectionID", "Name");
            return View();
        }

        // POST: LMS/Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeID,EmpCode,Name,Designation,Gender,ContactNo,Email,SupervisorID,IsSectionHead,SectionID,JoiningDate,ContractStartDate,ContractEndDate")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SectionID"] = new SelectList(_context.Section.Where(a => a.SectionID < 10), "SectionID", "Name", employee.SectionID);
            return View(employee);
        }

        // GET: LMS/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Inbox = Inbox;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            if(employee.Gender == "Male")
            {
                ViewData["Gender"] = new List<SelectListItem>
                {
                    new SelectListItem {Text = "Male", Value = "Male"},
                    new SelectListItem {Text = "Female", Value = "Female"}
                };
            }
            else
            {
                ViewData["Gender"] = new List<SelectListItem>
                {                
                    new SelectListItem {Text = "Female", Value = "Female"},
                    new SelectListItem {Text = "Male", Value = "Male"}
                };
            }
            ViewData["SectionID"] = new SelectList(_context.Section, "SectionID", "Name", employee.SectionID);
            ViewData["SupervisorID"] = new SelectList(_context.Employee.Where(a => a.SectionID == employee.SectionID || a.SectionID == 10), "SectionID", "Name");
            return View(employee);
        }

        // POST: LMS/Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeID,EmpCode,Name,Designation,Gender,ContactNo,Email,SupervisorID,IsSectionHead,SectionID,JoiningDate,ContractStartDate,ContractEndDate")] Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeID))
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
            ViewData["SectionID"] = new SelectList(_context.Section, "SectionID", "Name", employee.SectionID);
            return View(employee);
        }

        // GET: LMS/Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Section)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: LMS/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeID == id);
        }
    }
}
