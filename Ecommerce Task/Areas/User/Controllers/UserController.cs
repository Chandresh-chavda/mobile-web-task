using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAccess;

using Models;

namespace Ecommerce_Task.Areas.User.Controllers
{
    [Area("User")]
	public class UserController : Controller
	{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        //private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(
                    UserManager<ApplicationUser> userManager,
                    RoleManager<IdentityRole> roleManager)
        {

            _userManager = userManager;
            _roleManager = roleManager;

        }

        [HttpGet]
        public IActionResult DealerRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DealerRegister(DealerRegistration obj)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = obj.Name,
                    Email = obj.Email,
                    Name = obj.Name,
                    PhoneNumber = obj.Phonenumber,
                    State = obj.State,
                    PostalCode = obj.PostalCode,
                    IsActive = true,
                };
                await _userManager.CreateAsync(user, obj.Password);

                if (!_roleManager.RoleExistsAsync(Roles.role.Dealer.ToString()).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(Roles.role.Dealer.ToString())).GetAwaiter().GetResult();
                   
                }
                
                await _userManager.AddToRoleAsync(user, Roles.role.Dealer.ToString());
                TempData["success"] = "Registration success";

                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
           
            return View(obj);
            
        }
        [HttpGet]
        public IActionResult AdminRegister()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AdminRegisterasync(DealerRegistration obj)
        {

            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = obj.Name,
                    Email = obj.Email,
                    Name = obj.Name,
                    PhoneNumber = obj.Phonenumber,
                    State = obj.State,
                    status=Status.status.Approved,
                    PostalCode = obj.PostalCode,
                    IsActive = true,
                };
                await _userManager.CreateAsync(user, obj.Password);

                
                    _roleManager.CreateAsync(new IdentityRole(Roles.role.Admin.ToString())).GetAwaiter().GetResult();

                

                await _userManager.AddToRoleAsync(user, Roles.role.Admin.ToString());

                TempData["success"] = "Registration success";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            return View("AdminRegister");

        }

    }
}

