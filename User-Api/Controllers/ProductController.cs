using Ecommerce_Task.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using static System.Net.Mime.MediaTypeNames;

namespace User_Interface.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : Controller
    {

        private readonly ApplicationDbContext _db;
        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult GetAllProdct()
        {
            IEnumerable<AddProductDto> productList;

            productList = from product in _db.Products
                          where product.IsActive == true
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
                              ImageUrl=  product.ImageUrl,
							 

						  });


            return Ok(productList);
        }
        [HttpGet]
        public IActionResult GetProductById(int id)
        {
            IEnumerable<AddProductDto> productList;

            productList = from product in _db.Products
                          where product.IsActive == true && product.ProductID == id 
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
            return Ok(productList);

        }
        
    }
}
