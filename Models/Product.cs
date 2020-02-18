using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDataScraper.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Kind { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public string RatingUnit { get; set; }
        public string ImageUrl { get; set; }
    }
}
