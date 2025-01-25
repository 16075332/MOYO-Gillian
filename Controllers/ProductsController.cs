using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Assignment3_Backend.Models;
using Assignment3_Backend.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Assignment3_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository _storeRepo;

        public ProductsController(IRepository storeRepository)
        {
            _storeRepo = storeRepository;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var results = await _storeRepo.GetAllProductsAsync();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }
        [HttpGet]
        [Route("GetProductTypes")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllProductTypes()
        {
            try
            {
                var results = await _storeRepo.GetAllProdTypesAsync();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }
        [HttpGet]
        [Route("GetBrands")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllBrands()
        {
            try
            {
                var results = await _storeRepo.GetAllBrands();
                return Ok(results);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error. Please contact support.");
            }
        }

        [HttpPost]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductViewModel prodVm)
        {
            var date = DateTime.Now;
            string formattedDateTime = date.ToString("yyyy-MM-dd HH:mm:ss.fffffff");

            var course = new Product {
                Name = prodVm.name,
                Price = prodVm.price,
                Description = prodVm.description,
                ProductTypeId = prodVm.producttype,
                BrandId = prodVm.brand,
                Image = prodVm.image,
                DateCreated = Convert.ToDateTime(formattedDateTime)
            };

            try
            {
                _storeRepo.Add(course);
                await _storeRepo.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("Invalid transaction");
            }

            return Ok(course);
        }


    }
}
