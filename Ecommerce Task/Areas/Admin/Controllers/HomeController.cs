using DataAccess;
using Ecommerce_Task.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Models;
using System.Net.Mail;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static Models.Roles;

namespace Ecommerce_Task.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;       
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
                    UserManager<ApplicationUser> userManager,
                    RoleManager<IdentityRole> roleManager,
                    SignInManager<ApplicationUser> signInManager,
                    ApplicationDbContext db,
                    IHttpContextAccessor httpContextAccessor
                    )
        {
           
            _userManager= userManager;
            _roleManager= roleManager;
            _db = db;
            _signInManager= signInManager;
            _httpContextAccessor = httpContextAccessor;

        }
        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // User is logged in, than display dashboard
                return RedirectToAction("Dashboard","Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Login login)
        {
            
            var user =await _userManager.FindByEmailAsync(login.Email);
            if(user == null)
            {
                TempData["error"] = "Invalid UserName";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }

            
            var roleType =await _userManager.GetRolesAsync(user);
            if (user.status == Status.status.Pendind || user.IsActive==false)
            {
                TempData["error"] = "your request is not accepted yet";
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                
                    var result=await _signInManager.PasswordSignInAsync(user.UserName, login.Password, false, false);
                    
                        if (result.Succeeded)
                        {
                            HttpContext.Session.SetString("userId", user.Name);
                            TempData["success"] = "Login success";
                            return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
                        }
                        else
                        {
                        TempData["error"] = "Invalid Password";
                    }
                    
                
            }
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            TempData["success"] = "Logout success";
            return RedirectToAction("index", "home");
        }
       
        [HttpGet]
        public IActionResult Dashboard(string? searchItem)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // User is logged in, do something
                return View("PageNotFound");
            }
            
            DashBoardViewModel userList = new DashBoardViewModel();
           
            var roleClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim.Value == "SuperAdmin")
            {
                if (string.IsNullOrWhiteSpace(searchItem))
                {
                    userList.dashboards = (from user in _db.ApplicationUsers
                                join userrole in _db.UserRoles on user.Id equals userrole.UserId
                                join role in _db.Roles on userrole.RoleId equals role.Id
                                where role.Name != "SuperAdmin"
                                select (new Dashboard
                                {
                                    Id = user.Id,
                                    Name = user.Name,
                                    Email = user.Email,
                                    Role = role.Name,
                                    status = user.status.ToString(),
                                    Reason = user.Reason


                                })).ToList();
                  
                    userList.SearchItem= searchItem;
                    
                    


                }
                else
                {
                    Status status=new Status();
                    userList.dashboards = (from user in _db.ApplicationUsers
                                join userrole in _db.UserRoles on user.Id equals userrole.UserId
                                join role in _db.Roles on userrole.RoleId equals role.Id
                                where role.Name != "SuperAdmin" && role.Name.Contains(searchItem) || user.Name.Contains(searchItem) || user.Email.Contains(searchItem)
                              
                                select (new Dashboard
                                {
                                    Id = user.Id,
                                    Name = user.Name,
                                    Email = user.Email,
                                    Role = role.Name,
                                    status = user.status.ToString(),
                                    Reason = user.Reason


                                }));
                    userList.SearchItem = searchItem;

                }
            }
            if (roleClaim.Value == "Admin")
            {
                if (string.IsNullOrWhiteSpace(searchItem))
                {
                    userList.dashboards = (from user in _db.ApplicationUsers
                                           join userrole in _db.UserRoles on user.Id equals userrole.UserId
                                           join role in _db.Roles on userrole.RoleId equals role.Id
                                           where role.Name != "SuperAdmin" && role.Name != "Admin"
                                           select (new Dashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               Email = user.Email,
                                               Role = role.Name,
                                               status = user.status.ToString(),
                                               Reason = user.Reason


                                           })).ToList();

                    userList.SearchItem = searchItem;




                }
                else
                {

                    userList.dashboards = (from user in _db.ApplicationUsers
                                           join userrole in _db.UserRoles on user.Id equals userrole.UserId
                                           join role in _db.Roles on userrole.RoleId equals role.Id
                                           where role.Name != "SuperAdmin" && role.Name != "Admin" && role.Name.Contains(searchItem) 
                                           || user.Name.Contains(searchItem) && role.Name != "SuperAdmin" && role.Name != "Admin" ||
                                            role.Name != "SuperAdmin" && role.Name != "Admin" && user.Email.Contains(searchItem)
                                           select (new Dashboard
                                           {
                                               Id = user.Id,
                                               Name = user.Name,
                                               Email = user.Email,
                                               Role = role.Name,
                                               status = user.status.ToString(),
                                               Reason = user.Reason

                                           }));
                }
            }
            if (roleClaim.Value == "Dealer")
            {
               
                return RedirectToAction("Index", "Product", new { area = "User" });
               
            }


            return View(userList);
        }

        //for superadmin
        public async Task <IActionResult> registerasync()
        {
            ApplicationUser user = new ApplicationUser() {
                UserName = "Admin",
                Email = "chandresh.chavda18427@marwadieducation.edu.in",
                //PasswordHash = "Ck@123",
                Name = "Admin",
                EmailConfirmed = false,
                PhoneNumber = "9725145109",
                State = "Gujrat",
                LockoutEnabled = false,
                PostalCode = "361210",
                IsActive = true,
                SecurityStamp = "7ELAPF7ZYE6ZLLV4DZEZ3O7QDCUHNOZE",
                ConcurrencyStamp = "cc63bee6-6da9-4696-9054-2f51662d5f3c",
                status = Status.status.Approved


            };
            await _userManager.CreateAsync(user, user.PasswordHash= "Ck@123");

            if (!_roleManager.RoleExistsAsync(Roles.role.SuperAdmin.ToString()).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Roles.role.SuperAdmin.ToString())).GetAwaiter().GetResult();
           
            }
           

            IdentityResult result =await _userManager.AddToRoleAsync(user, Roles.role.Admin.ToString());
            

            return View();
        }

        [HttpGet]
       public async Task<IActionResult> ChangeStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user.status == Status.status.Pendind)
            {
                user.status = Status.status.Approved;
                
                await _db.SaveChangesAsync();



                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(user.Email)); // set the user email address
                message.From = new MailAddress("ckchavda077@gmail.com"); // set the admin email address
                message.Subject = "Registration Success";
                message.Body = $"WelCome {user.Name}, Your are successfuly Registered ";

                // Set up the SMTP client
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("ckchavda077@gmail.com", "iokcfszebedwosug"); // set the admin login credentials

                try
                {
                    client.Send(message);
                    ViewBag.Message = "Email sent successfully!";
                }
                catch (SmtpException ex)
                {
                    ViewBag.Message = "Error: " + ex.Message;
                }


                return RedirectToAction("Dashboard", "Home", new { area = "Admin" });

            }
            if (user.status == Status.status.Approved)
            {
                user.status = Status.status.Block;
                user.IsActive= false;
                await _db.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Home", new { area = "Admin" });

            }
            if (user.status == Status.status.Block)
            {
                user.status = Status.status.Approved;
                user.IsActive = true;
                await _db.SaveChangesAsync();
                return RedirectToAction("Dashboard", "Home", new { area = "Admin" });

            }
            return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
        }
        [HttpPost]
        public async Task<IActionResult> Rejectasync(string email, string Reason)
        {
           
            var user = await _userManager.FindByEmailAsync(email);
           
            user.status = Status.status.Reject;
            user.IsActive = false;
            user.Reason= Reason;
            await _db.SaveChangesAsync();



            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(email)); // set the user email address
            message.From = new MailAddress("ckchavda077@gmail.com"); // set the admin email address
            message.Subject = "Request Cancel";
            message.Body = Reason;

            // Set up the SMTP client
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("ckchavda077@gmail.com", "iokcfszebedwosug"); // set the admin login credentials

            try
            {
                client.Send(message);
                ViewBag.Message = "Email sent successfully!";
            }
            catch (SmtpException ex)
            {
                ViewBag.Message = "Error: " + ex.Message;
            }



            return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
        }
        [HttpGet]
        public IActionResult Popup(string email)
        {
            ViewBag.Email = email;
            return PartialView("_Popup");
        }

        [HttpGet]
        public IActionResult AddRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRole(AddRole obj)
        {

            if (ModelState.IsValid)
            {

                
                if(await _roleManager.RoleExistsAsync(obj.Role))
                {
                    TempData["error"] = "Role already Exist";

                }
                else
                {
                    _roleManager.CreateAsync(new IdentityRole(obj.Role.ToString())).GetAwaiter().GetResult();
                }           
                

                return RedirectToAction("AddRole", "Home", new { area = "Admin" });
            }

            return View(obj);

        }
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}
