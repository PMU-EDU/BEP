using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BES.Areas.LMS.Models;
using BES.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BES.Areas.LMS.Controllers
{
    [Area("LMS")]
    public class LeaveSummariesController : BaseController
    {        
        public LeaveSummariesController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor) { }

        // GET: LMS/EmpLeaveSummaries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LeaveSummaries.OrderByDescending(a=>a.Section);
            return View(await applicationDbContext.ToListAsync());
        }
       
      
    }
}