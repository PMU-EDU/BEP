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
using BES.API;
using System.IO;

namespace BES.Areas.LMS.Controllers
{
    [Area("LMS")]
    public class LeaveRequestsController : BaseController
    {           
        public LeaveRequestsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
        {                   
        }
        public async Task<IActionResult> IndexRefused()
        {           
            ViewBag.Inbox = Inbox;
            ViewBag.refused = Refused;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;                        
            var applicationDbContext = _context.LeaveRequest.Include(l => l.LeaveType).Include(e => e.Employee.Section).Where(a => a.ApprovedByHR == 2);
            return View(await applicationDbContext.ToListAsync());            
        }
        // GET: LMS/LeaveRequests
        public async Task<IActionResult> Index(int id)
        {
            ViewBag.Id = id;
            if(id == 0)
            {
                ViewBag.Text = "List of Pending Requests";
            }
            else if(id == 1)
            {
                ViewBag.Text = "List of Accepted Requests";
            }
            else
            {
                ViewBag.Text = "List of Rejected Requests";
            }
            ViewBag.Inbox = Inbox;
            ViewBag.refused = Refused;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;
            if (IsSpervisor)
            {
                var applicationDbContext = _context.LeaveRequest.Include(l => l.LeaveType).Include(e => e.Employee).Where(a => a.Employee.SectionID == SectionID && a.ApprovedBySection == id && a.EmployeeID != EmployeeID);
                return View(await applicationDbContext.ToListAsync());
            }
            else if(IsHRAdmin)
            {
                var applicationDbContext = _context.LeaveRequest.Include(l => l.LeaveType).Include(e => e.Employee.Section).Where(a => a.ApprovedByHR == id);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.LeaveRequest.Include(l => l.LeaveType).Include(e => e.Employee.Section).Where(a => a.ApprovedBySection == 1 && a.ApprovedByPD == id);
                return View(await applicationDbContext.ToListAsync());
            }          
           
        }

        // GET: LMS/LeaveRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequest                
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.LeaveRequestID == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }
        public ActionResult UserNotFound()
        {
            return View();
        }
        public ActionResult LeaveSummaryForHR(int id)
        {
            var leaverequests = _context.LeaveRequest.Include(l => l.LeaveType).Where(a => a.EmployeeID == id).ToList();
            // = leaverequests.Where(a=>a.ApprovedByPD == 0 || a.ApprovedBySection == 0).Sum(a=>a.TotalDays);
            var empLeaveSummary = _context.EmpLeaveSummary.Where(e => e.EmployeeID == id).ToList();
            ViewBag.Remaning = (empLeaveSummary.Sum(a => a.Total) - empLeaveSummary.Where(a => a.LeaveTypeID < 3).Sum(a => a.Availed)) - empLeaveSummary.Where(a => a.LeaveTypeID == 3).Sum(a => a.Total) - empLeaveSummary.Sum(a => a.Pending);
            ViewBag.Availed = empLeaveSummary.Sum(a => a.Availed);
            ViewBag.Pending = empLeaveSummary.Sum(a => a.Pending);
            var tuple = new Tuple<IEnumerable<LeaveRequest>, IEnumerable<EmpLeaveSummary>>(leaverequests, empLeaveSummary);
            return View(tuple);
        }
        public ActionResult LeaveSummary()
        {
            if (!IsAccountExit)
            {
                ViewBag.AccountNotExist = 1;
                return RedirectToAction(nameof(UserNotFound));
            }           
            ViewBag.Inbox = Inbox;
            ViewBag.Refused = Refused;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            if (IsPD)
            {
                return RedirectToAction(nameof(Index) , new { id = 0});
            }
            if (IsHRAdmin)
            {
                return RedirectToAction("Index","Employees");
            }
            var leaverequests = _context.LeaveRequest.Include(l => l.LeaveType).Where(a => a.EmployeeID == EmployeeID).ToList();
            // = leaverequests.Where(a=>a.ApprovedByPD == 0 || a.ApprovedBySection == 0).Sum(a=>a.TotalDays);
            var empLeaveSummary = _context.EmpLeaveSummary.Where(e=>e.EmployeeID == EmployeeID).ToList();           
            ViewBag.Remaning = (empLeaveSummary.Sum(a => a.Total) - empLeaveSummary.Where(a=>a.LeaveTypeID < 3).Sum(a => a.Availed)) - empLeaveSummary.Where(a=>a.LeaveTypeID == 3).Sum(a=>a.Total) - empLeaveSummary.Sum(a => a.Pending);
            ViewBag.Availed = empLeaveSummary.Sum(a => a.Availed);
            ViewBag.Pending = empLeaveSummary.Sum(a=>a.Pending);
            var tuple = new Tuple<IEnumerable<LeaveRequest>, IEnumerable<EmpLeaveSummary>>(leaverequests, empLeaveSummary);
            return View(tuple);
        }
        public ActionResult LeaveSummaryOf(int id)
        {            
            ViewBag.Inbox = Inbox;
            ViewBag.Refused = Refused;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            ViewBag.IsPD = IsPD ? 1 : 0;
            var leaverequest = _context.LeaveRequest.Include(l => l.LeaveType).Include(e=>e.Employee).Where(a => a.LeaveRequestID == id).FirstOrDefault();
            var leaverequests = _context.LeaveRequest.Include(l => l.LeaveType).Where(a => a.EmployeeID == EmployeeID).ToList();
            //ViewBag.Pending = leaverequests.Where(a => a.ApprovedByPD == 0 || a.ApprovedBySection == 0).Sum(a => a.TotalDays);
            var empLeaveSummary = _context.EmpLeaveSummary.Where(e => e.EmployeeID == leaverequest.EmployeeID).ToList();
            ViewBag.Remaning = (empLeaveSummary.Sum(a => a.Total) - empLeaveSummary.Where(a => a.LeaveTypeID < 3).Sum(a => a.Availed)) - empLeaveSummary.Where(a => a.LeaveTypeID == 3).Sum(a => a.Total) - empLeaveSummary.Sum(a => a.Pending);
            ViewBag.Availed = empLeaveSummary.Sum(a => a.Availed);
            ViewBag.Pending = empLeaveSummary.Sum(a => a.Pending);
            var tuple = new Tuple< IEnumerable<EmpLeaveSummary>, LeaveRequest>(empLeaveSummary,leaverequest);
            return View(tuple);
        }
        [HttpPost]
        public JsonResult SubmitLeave(int id, short val, string remarks, int emplyeeID)
        {
            LeaveRequest Obj = _context.LeaveRequest.Find(id);
            if (IsSpervisor)
            {
                Obj.ApprovedBySection = val;
                Obj.ApprovedBySectionDate = DateTime.Now.Date;
                Obj.SupervisorRemarks = remarks;
            }
            if (IsHRAdmin)
            {
                Obj.ApprovedByHR = val;
                Obj.ApprovedByHRDate = DateTime.Now.Date;
                Obj.HRRemarks = remarks;
            }
            if (IsPD)
            {
                Obj.ApprovedByPD = val;
                Obj.ApprovedByPDDate = DateTime.Now.Date;
                Obj.PDRemarks = remarks;
                EmpLeaveSummary leaveSummaryObj = _context.EmpLeaveSummary.Where(a => a.EmployeeID == emplyeeID && a.LeaveTypeID == id).FirstOrDefault();
                leaveSummaryObj.Pending = 0;
                if(val == 2)
                {
                    leaveSummaryObj.Availed = leaveSummaryObj.Availed - Obj.TotalDays;
                }
                _context.Update(leaveSummaryObj);
                _context.SaveChanges();
            }
            _context.Update(Obj);
            _context.SaveChanges();
            return Json("Success");
        }
        // GET: LMS/LeaveRequests/Create
        public IActionResult Create()
        {
            ViewBag.Inbox = Inbox;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.refused = Refused;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            ViewBag.IsHRAdmin = IsHRAdmin ? 1 : 0;
            var loginName = User.Identity.Name;
            var employee = _context.Employee.Where(a => a.Name == loginName).FirstOrDefault();
            ViewBag.SectionHead = _context.Employee.Where(a => a.EmployeeID == employee.SupervisorID).Select(a=>a.Name).FirstOrDefault();
            ViewBag.Name = employee.Name;

            var empSummary = _context.EmpLeaveSummary.Where(a => a.EmployeeID == employee.EmployeeID).ToList();
            ViewBag.ABalance = empSummary.Where(a => a.LeaveTypeID == 1).Max(a => a.Total) - empSummary.Where(a => a.LeaveTypeID == 1).Max(a => a.Availed) - empSummary.Where(a => a.LeaveTypeID == 1).Max(a => a.Pending);
            ViewBag.SBalance = empSummary.Where(a => a.LeaveTypeID == 2).Max(a => a.Total) - empSummary.Where(a => a.LeaveTypeID == 2).Max(a => a.Availed) - empSummary.Where(a => a.LeaveTypeID == 2).Max(a => a.Pending);            
            ViewBag.Designation = employee.Designation;            
            ViewData["LeaveTypeID"] = new SelectList(_context.LeaveType.Where(a=> a.LeaveTypeID < 3), "LeaveTypeID", "Name");
            ViewData["OnBehalfOfName"] = new SelectList(_context.Employee.Where(a => a.SectionID == SectionID && a.EmployeeID != EmployeeID), "EmployeeID", "Name");
            return View();
        }

        // POST: LMS/LeaveRequests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveRequestID,DateFrom,DateTo,TotalDays,EmployeeID,LeaveTypeID,Remarks,ApprovedBySection,ApprovedBySectionDate,SupervisorRemarks,ApprovedByPD,ApprovedByPDDate,PDRemarks,Nomination,NominatedID,OnBehalfOf,IsMedicalCertificateRequired,MedicalCertificatePath,AppliedDate")] LeaveRequest leaveRequest, short Days, IFormFile Attachment)
        {
            if (ModelState.IsValid)
            {                
                //-------------
                var empSummaryObj = _context.EmpLeaveSummary.Where(a => a.EmployeeID == EmployeeID && a.LeaveTypeID == leaveRequest.LeaveTypeID).FirstOrDefault();
                empSummaryObj.Pending = empSummaryObj.Availed + Days;
                empSummaryObj.Pending = empSummaryObj.Pending + Days;
                _context.Update(empSummaryObj);
                //-------------
                leaveRequest.AppliedDate = DateTime.Now.Date;
                leaveRequest.TotalDays = Days;
                leaveRequest.EmployeeID = EmployeeID;
                if (IsSpervisor)
                {
                    leaveRequest.ApprovedBySection = 1;
                    leaveRequest.ApprovedBySectionDate = DateTime.Now.Date;
                }
                //-----------------------Attachment------------------------
                if (Attachment != null)
                {
                    var rootPath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot\\Documents\\HR\\");
                    string fileName = Path.GetFileName(Attachment.FileName);
                    fileName = fileName.Replace("&", "n");
                    string Code = _context.Employee.Include(a => a.Section).Where(a=>a.EmployeeID == leaveRequest.EmployeeID).Select(a => a.Section.Name).FirstOrDefault() + "_EID_" + leaveRequest.EmployeeID + leaveRequest.AppliedDate.ToString("ddMMyyyy");

                    leaveRequest.MedicalCertificatePath = Path.Combine("/Documents/HR/", Code);//Server Path                
                    string sPath = Path.Combine(rootPath + Code);
                    if (!System.IO.Directory.Exists(sPath))
                    {
                        System.IO.Directory.CreateDirectory(sPath);
                    }
                    string FullPathWithFileName = Path.Combine(sPath, fileName);
                    using (var stream = new FileStream(FullPathWithFileName, FileMode.Create))
                    {
                        await Attachment.CopyToAsync(stream);
                    }
                }
                //-----------------------END-------------------------------
                _context.Add(leaveRequest);
                await _context.SaveChangesAsync();
                //-------------SMS--------------------------------
                string msg = "HR-LMS\n Days("+ Days.ToString() +") " + _context.LeaveType.Where(a=>a.LeaveTypeID == leaveRequest.LeaveTypeID).Select(a=>a.Name).FirstOrDefault() + "leave required for approval in your MIS inbox.\nVisit http://eu.bep.org.pk";
                ZongSMS ObjSMS = new ZongSMS();
                //var contacts = _context.Contact.Where(a => a.IsActive == true).ToList();
                //foreach (var contact in contacts)
                //{
                //    ObjSMS.SendSingleSMS(msg, contact.ContactNumber);
                //}
                ObjSMS.SendSingleSMS(msg, "923337905929");
                ObjSMS.SendSingleSMS(msg, "923327822567");
                //-------------END SMS----------------------------
                return RedirectToAction(nameof(LeaveSummary));
            }
            ViewBag.Inbox = Inbox;
            ViewBag.Accepted = AcceptedRequests;
            ViewBag.Rejected = RejectedRequests;
            ViewBag.IsSupervisor = IsSpervisor ? 1 : 0;
            var loginName = User.Identity.Name;
            var employee = _context.Employee.Where(a => a.Name == loginName).FirstOrDefault();
            ViewBag.SectionHead = _context.Employee.Where(a => a.EmployeeID == employee.SupervisorID).Select(a => a.Name).FirstOrDefault();
            ViewBag.Name = employee.Name;

            var empSummary = _context.EmpLeaveSummary.Where(a => a.EmployeeID == employee.EmployeeID).ToList();
            ViewBag.ABalance = empSummary.Where(a => a.LeaveTypeID == 1).Max(a => a.Total) - empSummary.Where(a => a.LeaveTypeID == 1).Max(a => a.Availed);
            ViewBag.SBalance = empSummary.Where(a => a.LeaveTypeID == 2).Max(a => a.Total) - empSummary.Where(a => a.LeaveTypeID == 2).Max(a => a.Availed);
            ViewBag.Designation = employee.Designation;            
            ViewData["LeaveTypeID"] = new SelectList(_context.LeaveType.Where(a => a.LeaveTypeID > 0 && a.LeaveTypeID < 3), "LeaveTypeID", "Name", leaveRequest.LeaveTypeID);
            ViewData["OnBehalfOfName"] = new SelectList(_context.Employee.Where(a => a.SectionID == SectionID && a.EmployeeID != EmployeeID), "EmployeeID", "Name");
            return View(leaveRequest);
        }

        // GET: LMS/LeaveRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }            
            ViewData["LeaveTypeID"] = new SelectList(_context.LeaveType, "LeaveTypeID", "LeaveTypeID", leaveRequest.LeaveTypeID);
            return View(leaveRequest);
        }

        // POST: LMS/LeaveRequests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeaveRequestID,DateFrom,DateTo,TotalDays,EmployeeID,LeaveTypeID,Remarks,ApprovedBySection,ApprovedBySectionDate,SupervisorRemarks,ApprovedByPD,ApprovedByPDDate,PDRemarks,Nomination,NominatedID,OnBehalfOf,IsMedicalCertificateRequired,MedicalCertificatePath,AppliedDate")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveRequestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRequestExists(leaveRequest.LeaveRequestID))
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
            ViewData["LeaveTypeID"] = new SelectList(_context.LeaveType, "LeaveTypeID", "LeaveTypeID", leaveRequest.LeaveTypeID);
            return View(leaveRequest);
        }

        // GET: LMS/LeaveRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequest            
                .Include(l => l.LeaveType)
                .FirstOrDefaultAsync(m => m.LeaveRequestID == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // POST: LMS/LeaveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveRequest = await _context.LeaveRequest.FindAsync(id);
            _context.LeaveRequest.Remove(leaveRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequest.Any(e => e.LeaveRequestID == id);
        }
    }
}
