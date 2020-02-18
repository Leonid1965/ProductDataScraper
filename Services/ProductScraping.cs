using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProductDataScraper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDataScraper.Services
{
    public class ProductScraping : IProductScraping
    {
        readonly IConfiguration config;

        public ProductScraping(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<Product[]> GetData()
        {
            string url = config["Products:Url"];

           // HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
            //HtmlDocument doc = await web.LoadFromWebAsync(url);

            var doc = new HtmlDocument();
            doc.Load(@"C:\tmp\Products Page\Amazon Best Sellers  Best C# Programming.htm");


            HtmlNode htmlNode = null; HtmlNode item = null;
            string txt;
            var nodes = doc.DocumentNode.SelectNodes("//div[@class='a-section a-spacing-none aok-relative']").ToList();

            Product[] products = new Product[nodes.Count];
            Product product = null;

            
            //
            for (int i = 0; i < nodes.Count; i++)
            {
                try
                {
                    item = nodes[i];
                    product = new Product
                    {
                        Id = Guid.NewGuid().ToString()
                    };

                    product.Name = "book";
                    htmlNode = item.ChildNodes["span"].ChildNodes["a"].ChildNodes["span"].ChildNodes["div"].ChildNodes["img"];
                    product.ImageUrl = htmlNode.Attributes["src"].Value;
                    product.Title = htmlNode.Attributes["alt"].Value;

                    if (item.ChildNodes["span"].ChildNodes["div"].ChildNodes["a"] != null)
                        product.Author = item.ChildNodes["span"].ChildNodes["div"].ChildNodes["a"].InnerText;
                    else
                        product.Author = item.ChildNodes["span"].ChildNodes["div"].ChildNodes["span"].InnerText;
                    if (item.ChildNodes["span"].ChildNodes.Count >= 6)
                        product.Kind = item.ChildNodes["span"].ChildNodes[5].ChildNodes["span"].InnerText;
                    else
                        product.Kind = item.ChildNodes["span"].ChildNodes[2].ChildNodes["span"].InnerText;

                    htmlNode = item.SelectNodes("//span[@class='p13n-sc-price']").FirstOrDefault();
                    product.Price = double.Parse(htmlNode.InnerText.Substring(1));
                    product.Currency = htmlNode.InnerText.Substring(0, 1);
                    product.RatingUnit = "out of 5 stars";

                    if (item.ChildNodes["span"].ChildNodes[3].ChildNodes["a"].Attributes["title"] != null) 
                    {
                        txt = item.ChildNodes["span"].ChildNodes[3].ChildNodes["a"].Attributes["title"].Value;
                        product.Rating = double.Parse(txt.Split(" ")[0]);
                    }

                    products[i] = product;
                    item.Remove(); //correct document, error is found.
                }
                catch ( Exception e)
                {
                    throw new Exception("ProductScraping.GetData: " + e.Message);
                }
            }

            return products;
        }

    }
}
