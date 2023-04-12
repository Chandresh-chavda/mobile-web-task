
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using System.Diagnostics;


namespace User_Interface.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<AddProductDto> reservationList = new List<AddProductDto>();
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                return true;
            };
            using (var httpClient = new HttpClient(handler))
            {
                using (var response = await httpClient.GetAsync("https://localhost:7142/api/Product/GetAllProdct"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    reservationList = JsonConvert.DeserializeObject<List<AddProductDto>>(apiResponse);
                }
            }
            

            return View(reservationList);

        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}