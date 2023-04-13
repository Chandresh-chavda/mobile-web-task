using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using User_Interface.Models;

namespace User_Interface.Controllers
{
	public class AuthenticationController : Controller
    {
        private readonly HttpClient _httpClient;
		
		public AuthenticationController(HttpClient httpClient)
        {
                _httpClient=httpClient;
				

		}
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Login obj)
        {
            var data = JsonConvert.SerializeObject(obj);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
			var handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
			{
				// Always return true to bypass certificate validation
				return true;
			};

			// Create HttpClient with custom handler
			using (var httpClient = new HttpClient(handler))
			{
				var uri = "https://localhost:7142/api/Authentication/Login";
				var response = await httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    GlobalUserName.MyGlobalUser = obj.Email;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["error"] = "Invalid Email Password";
                    return View();
                }
				return View();
			}

			
        }
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> Register(DealerRegistration obj)
        {
            var data = JsonConvert.SerializeObject(obj);
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                // Always return true to bypass certificate validation
                return true;
            };

            using (var httpClient = new HttpClient(handler))
            {
                var uri = "https://localhost:7142/api/Authentication/Registration";
                var response = await httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["success"] = "Registration success";

                    return RedirectToAction("Login", "Authentication");
                }
                else
                {
                    TempData["error"] = "something wrong";

                    return RedirectToAction("Register", "Authentication");
                }

            }
        }

        public async Task<IActionResult> LogOut()
        {
            GlobalUserName.MyGlobalUser = null;
            return RedirectToAction("Index", "Home");
        }



    }
}

