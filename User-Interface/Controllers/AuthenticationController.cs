using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;

namespace User_Interface.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Login obj)
        {
           
           
            return View();
        }
    }
}
