using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductDataScraper.Models;

namespace ProductDataScraper.Services
{
    public class ScraperDbContext : DbContext
    {
        public ScraperDbContext(DbContextOptions<ScraperDbContext> options): base(options)
        {
        }
        public DbSet<Product> products { get; set; }
        public DbSet<User> users { get; set; }
    }
}
