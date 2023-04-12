using Ecommerce_Task.Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Models;
using System.Security.AccessControl;

namespace Ecommerce_Task.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(ApplicationDbContext db,
            IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // User is logged in, do something
                return RedirectToAction("PageNotFound", "Home", new {area = ""});
            }
            IEnumerable<AddProductDto> products = null;
            var userId = "";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetUserId();
            }

            products = from product in _db.Products
					   where product.DealerId == userId
					   let lastDiscount = _db.discounts
                       .Where(d => d.ProductId == product.ProductID)
                       .OrderByDescending(d => d.Id).FirstOrDefault()
					   select (new AddProductDto
                       {

                           ProductID = product.ProductID,
                           DealerId = product.DealerId,
                           Name = product.Name,
                           Description = product.Description,
                           price = product.price,
                           Discount= lastDiscount == null ? 0.0 :lastDiscount.DiscountAmount,
                           Quentity = product.Quentity,
                           IsActive = product.IsActive,

                       });

            return View(products);
        }
        public IActionResult IndexForAdmin(string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // User is logged in, do something
                return RedirectToAction("PageNotFound", "Home", new { area = "" });
            }
            IEnumerable<AddProductDto> products = null;
            var userId = "";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetUserId();
            }

			products = from product in _db.Products
					   where product.DealerId == id
					   let lastDiscount = _db.discounts
					   .Where(d => d.ProductId == product.ProductID)
					   .OrderByDescending(d => d.Id).FirstOrDefault()
					   select (new AddProductDto
					   {

						   ProductID = product.ProductID,
						   DealerId = product.DealerId,
						   Name = product.Name,
						   Description = product.Description,
						   price = product.price,
						   Discount = lastDiscount == null ? 0.0 : lastDiscount.DiscountAmount,
						   Quentity = product.Quentity,
						   IsActive = product.IsActive,

					   });

			return View(products);
        }
        public IActionResult DeActive(int id)
        {
            var products = _db.Products.FirstOrDefault(u => u.ProductID == id);
            string dealerId = products.DealerId;
            if(products.IsActive == true)
            {
                products.IsActive = false;
            }
            else
            {
                products.IsActive = true;
            }
            _db.SaveChanges();
            return RedirectToAction("IndexForAdmin", new { id = dealerId });
           
        }
        public IActionResult AddProduct(int? id)
        {


            if (id == null)
            {
                return View();
            }
            else
            {
                AddProductDto productsData = null;
                productsData = (from pro in _db.Products
                                
                                where pro.ProductID == id
                                select (new AddProductDto
                                {

                                    ProductID = pro.ProductID,
                                    DealerId = pro.DealerId,
                                    Name = pro.Name,
                                    Description = pro.Description,
                                    price = pro.price,
                                    
                                    Quentity = pro.Quentity,
                                    IsActive = pro.IsActive

                                })).FirstOrDefault();

                return View(productsData);
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(AddProductDto pData, IFormFile? file)
        {
            var userId = "";
            if (User.Identity.IsAuthenticated)
            {
                userId = User.Identity.GetUserId();
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {


                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\product");
                    var extension = Path.GetExtension(file.FileName);

                    if (pData.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, pData.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }
                    pData.ImageUrl = @"\images\product\" + fileName + extension;
                }
                if (pData.ProductID == 0 || pData.ProductID == null)

                {
                    Product product = new Product
                    {
                        DealerId = userId,
                        Name = pData.Name,
                        Description = pData.Description,
                        price = pData.price,
                        Quentity = pData.Quentity,
                        IsActive = pData.IsActive,                       
                        CreatedON = pData.CreatedON,
                        UpdatedON = pData.UpdatedON,
                        ImageUrl = pData.ImageUrl


                    };
                    _db.Products.Add(product);
                    _db.SaveChanges();
                    TempData["success"] = "Product Added Successfully";
                    RedirectToAction("Index");
                }
                else if (pData.ProductID != 0)
                {
                    Product product = new Product
                    {
                        ProductID = (int)pData.ProductID,
                        DealerId = userId,
                        Name = pData.Name,
                        Description = pData.Description,
                        price = pData.price,
                        Quentity = pData.Quentity,
                        IsActive = pData.IsActive,                      
                        CreatedON = pData.CreatedON,
                        UpdatedON = pData.UpdatedON,
                        ImageUrl = pData.ImageUrl
                    };
                    _db.Products.Update(product);
                    _db.SaveChanges();
                    TempData["success"] = "Product Updated Successfully";
                    RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            return View(pData);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {

            var result = _db.Products.FirstOrDefault(u => u.ProductID == id);

            if (result == null)
            {
                return Json(new { success = false, Message = "Error While deleting" });
            }
            if (result.ImageUrl != null)
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, result.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _db.Products.Remove(result);
            _db.SaveChanges();
            TempData["success"] = "Product Deleted Successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult AddDiscount(int id)
        {
            var data = _db.Products.Find(id);
            Discount discount = new Discount
            {
                ProductId = data.ProductID,
                ProductName = data.Name,
                ProductPrise = data.price
            };
            return View(discount);
        }
        [HttpPost]
        public IActionResult AddDiscount(Discount discount)
        {
            if (discount.FromDate < DateTime.Now.Date)
            {
                ModelState.AddModelError("FromDate", "From Date Must be Today Date or Future Date");
            }
            if (discount.ToDate <= DateTime.Now)
            {
                ModelState.AddModelError("ToDate", "From Date Must be Today Date or Future Date");
            }
            else
            {

                if (discount.discountType == Discount.DiscountType.Percentage)
                {
                    if (discount.DiscountAmount > 100 || discount.DiscountAmount < 1)
                    {

                    }
                }
            }
            if (ModelState.IsValid)
            {

                if (discount.discountType == Discount.DiscountType.Rupee)
                {
                    if(discount.DiscountAmount > discount.ProductPrise )
                    {
						ModelState.AddModelError("DiscountAmount", "Discount Amount not Higher than Product Prise ");
						return View(discount);
					}
                    if (discount.DiscountAmount <= 0.0)
                    {
                        ModelState.AddModelError("DiscountAmount", "Discount Amount not be zero");
                        return View(discount);
                    }
                    _db.discounts.Add(discount);
                    _db.SaveChanges();
                    TempData["success"] = "Discount Added";

                }
                else
                {

                    var productPrise = _db.Products.FirstOrDefault(u => u.ProductID == discount.ProductId).price;
                    var discAmount = productPrise * discount.DiscountAmount / 100;
                    discount.DiscountAmount = discAmount;
                    _db.discounts.Add(discount);
                    _db.SaveChanges();
                    TempData["success"] = "Discount Added";

                }
				return RedirectToAction("Index");
			}
            
            return View(discount);

        }


    }
   
}
