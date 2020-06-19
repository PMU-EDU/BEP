using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BES.Data;
using BES.Models.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BES.Controllers.Reports
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            //_hostingEnvironment = hostingEnvironment;
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Index()
        {
            var users = from u in _context.Users
                        join r in _context.UserRoles on new { Id = u.Id } equals new { Id = r.UserId }
                        join Roles in _context.Roles on new { RoleId = r.RoleId } equals new { RoleId = Roles.Id }

                        select new UserView
                        {
                            Id = u.Id,
                            Email = u.Email,
                            Password = u.PhoneNumber,
                            PasswordHash = u.PasswordHash,
                            Role = Roles.Name,
                            Region = u.RegionalAccess
                        };
            return View(await users.ToListAsync());
        }

        [Authorize(Roles = "Administrator")]

        public async Task<IActionResult> Change(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            UserView userView = new UserView
            {
                Id = user.Id,
                Email = user.Email,
                Password = user.PhoneNumber,
                Name = user.UserName,
                PasswordHash = user.PasswordHash
            };
            return View(userView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(UserView userView)
        {
            if (ModelState.IsValid)
            {
                //var user = new ApplicationUser { Id= userView.Id, UserName = userView.Name, Email = userView.Email, PhoneNumber = userView.Password };
                //ApplicationUser user = _context.Users.Find(userView.Id);
                ApplicationUser user = await _userManager.FindByIdAsync(userView.Id);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, userView.Password);
                user.SecurityStamp = Guid.NewGuid().ToString();

                //UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(_context);
                //await store.SetPasswordHashAsync(user, user.PasswordHash);
                //await store.SetPhoneNumberAsync(user, user.PhoneNumber);
                //await store.UpdateAsync(user);

                await _userManager.ChangePasswordAsync(user, user.PasswordHash, userView.Password);
                await _userManager.ChangePhoneNumberAsync(user, userView.Password, "");
                await _userManager.SetPhoneNumberAsync(user, userView.Password);
                await _userManager.ResetPasswordAsync(user, code, userView.Password);
                await _userManager.UpdateAsync(user);

                // _context.Update(User);
                //_context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }
    }
}