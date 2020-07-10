using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BES.Areas.Procurement.Models;
using BES.Data;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace BES.Areas.Procurement.Controllers
{
    [Area("Procurement")]
    public class AddendumWorksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddendumWorksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Procurement/AddendumWorks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AddendumWorks.Include(a => a.ActivityDetailWork).Include(a => a.AddendumType);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> PartialIndex(short id)
        {
            var applicationDbContext = _context.AddendumWorks.Include(a => a.ActivityDetailWork.Activity).Include(a => a.AddendumType).Where(a => a.ActivityDetailWork.ActivityID == id);
            return PartialView(await applicationDbContext.ToListAsync());
        }
        // GET: Procurement/AddendumWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendumWorks = await _context.AddendumWorks
                .Include(a => a.ActivityDetailWork)
                .Include(a => a.AddendumType)
                .FirstOrDefaultAsync(m => m.AddendumId == id);
            if (addendumWorks == null)
            {
                return NotFound();
            }

            return View(addendumWorks);
        }

        // GET: Procurement/AddendumWorks/Create
        public IActionResult Create(short id)
        {
            ViewBag.AName = _context.Activity.Find(id).Name;
            short val = _context.ActivityDetail.Where(a => a.ActivityID == id && a.StepID == 21).Select(a => a.StepID).FirstOrDefault();
            if (val < 1)
            {
                ViewBag.Pending = "Before Award of contract, you are unable to add Addendum!";
            }
            ViewBag.AID = id;
            ViewData["AddendumTypeId"] = new SelectList(_context.AddendumType, "AddendumTypeId", "Name");
            AddendumWorks Obj = new AddendumWorks();            
            Obj.ActivityDetailWorkID = _context.ActivityDetailWork.Include(a=>a.Activity).Where(a => a.Activity.ActivityID == id).Select(a => a.ActivityDetailWorkID).FirstOrDefault();
            return View(Obj);
        }

        // POST: Procurement/AddendumWorks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(short id, [Bind("AddendumId,ActivityDetailWorkID,AddendumTypeId,Attachment,Remarks,ExpiryDate,CurrentDate,ActualAmount")] AddendumWorks addendumWorks, IFormFile Attachment)
        {
            if (ModelState.IsValid)
            {
                if (Attachment != null)
                {
                    var rootPath = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot\\Documents\\Procurement\\Works\\");
                    string fileName = Path.GetFileName(Attachment.FileName);
                    fileName = fileName.Replace("&", "n");
                    string AName = _context.ActivityDetailWork.Include(a=>a.Activity).Where(a => a.ActivityDetailWorkID == addendumWorks.ActivityDetailWorkID).Select(a => a.Activity.Name).FirstOrDefault().ToString();
                    AName = AName.Replace("&", "n");                    
                    int NextID = (_context.AddendumWorks.Max(a => (int?)a.AddendumId) ?? 1) + 1;                    
                    addendumWorks.Attachment = Path.Combine("/Documents/Procurement/Works/" + "//" + AName + "//Addendum//" + NextID.ToString() + "//" + fileName);//Server Path                
                    string sPath = Path.Combine(rootPath + "/" + AName + "//Addendum//" + NextID.ToString());
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
                addendumWorks.CurrentDate = DateTime.Now.Date;
                _context.Add(addendumWorks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Create), new { id });
            }
            ViewData["ActivityDetailWorkID"] = new SelectList(_context.ActivityDetailWork, "ActivityDetailWorkID", "ActivityDetailWorkID", addendumWorks.ActivityDetailWorkID);
            ViewData["AddendumTypeId"] = new SelectList(_context.AddendumType, "AddendumTypeId", "AddendumTypeId", addendumWorks.AddendumTypeId);
            return View(addendumWorks);
        }

        // GET: Procurement/AddendumWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendumWorks = await _context.AddendumWorks.FindAsync(id);
            if (addendumWorks == null)
            {
                return NotFound();
            }
            ViewData["ActivityDetailWorkID"] = new SelectList(_context.ActivityDetailWork, "ActivityDetailWorkID", "ActivityDetailWorkID", addendumWorks.ActivityDetailWorkID);
            ViewData["AddendumTypeId"] = new SelectList(_context.AddendumType, "AddendumTypeId", "AddendumTypeId", addendumWorks.AddendumTypeId);
            return View(addendumWorks);
        }

        // POST: Procurement/AddendumWorks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AddendumId,ActivityDetailWorkID,AddendumTypeId,Attachment,Remarks,ExpiryDate,CurrentDate,ActualAmount")] AddendumWorks addendumWorks)
        {
            if (id != addendumWorks.AddendumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(addendumWorks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddendumWorksExists(addendumWorks.AddendumId))
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
            ViewData["ActivityDetailWorkID"] = new SelectList(_context.ActivityDetailWork, "ActivityDetailWorkID", "ActivityDetailWorkID", addendumWorks.ActivityDetailWorkID);
            ViewData["AddendumTypeId"] = new SelectList(_context.AddendumType, "AddendumTypeId", "AddendumTypeId", addendumWorks.AddendumTypeId);
            return View(addendumWorks);
        }

        // GET: Procurement/AddendumWorks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var addendumWorks = await _context.AddendumWorks
                .Include(a => a.ActivityDetailWork)
                .Include(a => a.AddendumType)
                .FirstOrDefaultAsync(m => m.AddendumId == id);
            if (addendumWorks == null)
            {
                return NotFound();
            }

            return View(addendumWorks);
        }

        // POST: Procurement/AddendumWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var addendumWorks = await _context.AddendumWorks.FindAsync(id);
            _context.AddendumWorks.Remove(addendumWorks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AddendumWorksExists(int id)
        {
            return _context.AddendumWorks.Any(e => e.AddendumId == id);
        }
    }
}
