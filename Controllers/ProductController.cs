using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProductDataScraper.Models;
using ProductDataScraper.Services;
using sc = Security.Services.SecurityUtils;

namespace ProductDataScraper.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {

        private readonly ILogger<ProductController> _logger;
        private readonly IProductScraping scraping;
        private readonly IDbRepo dbRepo;

        public ProductController(ILogger<ProductController> logger, IProductScraping scraping, IDbRepo dbRepo)
        {
            _logger = logger;
            this.scraping = scraping;
            this.dbRepo = dbRepo;
        }

        //http://localhost:44329/product/all
        //https://localhost:44329/product
        [Route("All")]
        [HttpGet]
        public async Task<ActionResult<AppResponse<Product[]>>> All()
        {
            if (!sc.IsAuthed(this))
                return Unauthorized();

            AppResponse<Product[]> appResponse = null;
            try
            {
                Product[] products = await dbRepo.GetAll<Product>();
                string msg = products != null ? "" : "Products are not found";
                appResponse = new AppResponse<Product[]> { Result = products, ErrorMessage = msg };
            }
            catch (Exception e)
            {
                appResponse = new AppResponse<Product[]> { ErrorMessage = e.Message };
            }

            return appResponse;
        }

        //http://localhost:44329/product/ScrapeAndSave
        [AllowAnonymous]
        [Route("ScrapeAndSave")]
        [HttpPost]
        public async Task<ActionResult<AppResponse<string>>> ScrapeAndSave()
        {
            AppResponse<string> appResponse = null;
            try
            {
                string msg = "Products are already exist in the database."; string result = "";
                if (!(await dbRepo.DataExists<Product>()))
                {
                    Product[] products = await scraping.GetData();
                    await dbRepo.AddRange(products);
                    await dbRepo.SaveChanges();
                    msg = ""; result = "ok";
                }
                appResponse = new AppResponse<string> { Result = result, ErrorMessage = msg };
            }
            catch (Exception e)
            {
                appResponse = new AppResponse<string> { ErrorMessage = e.Message };
            }
            return appResponse;
        }

        //http://localhost:44329/product/get?id=0932343f-bcab-46b0-a956-2dac3ff56d42
        // [HttpGet("{id}")]
        [Route("item")]
        [HttpGet]
        public async Task<ActionResult<AppResponse<Product>>> GetItem(string id)
        {
            AppResponse<Product> appResponse = null;
            try
            {
                Product entity = await dbRepo.GetItem<Product>(id);
                string msg = entity != null ? "" : "Product is not found";
                appResponse = new AppResponse<Product> { Result = entity, ErrorMessage = msg };
            }
            catch (Exception e)
            {
                appResponse = new AppResponse<Product> {  ErrorMessage = e.Message };
            }

            return appResponse;
        }


    }
}
