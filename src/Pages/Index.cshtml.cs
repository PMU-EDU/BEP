using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BES.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BES.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

         public IndexModel(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
           // _logger = logger;
        }
        public void OnGet()
        {
           // var user =  _signInManager.UserManager.FindByNameAsync(User.Identity.Name);

           // var roles =  _signInManager.UserManager.GetRolesAsync(user);


        }
    }
}
