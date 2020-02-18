using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDataScraper.Services
{
    public class DbRepo : IDbRepo
    {
        private readonly ScraperDbContext scraperDbContext;

        public DbRepo(ScraperDbContext scraperDbContext)
        {
            this.scraperDbContext = scraperDbContext;
        }

        public async Task AddRange<T>(T[] entities, bool save = false) where T : class
        {
            await scraperDbContext.AddRangeAsync(entities);      
            if (save)
                await scraperDbContext.SaveChangesAsync();
        }

        public async Task<bool> DataExists<T>() where T : class
        {
            T entity = await scraperDbContext.Set<T>().FirstOrDefaultAsync();
            return (entity != null);
        }

        public async Task<T[]> GetAll<T>() where T : class
        {
            return await scraperDbContext.Set<T>().ToArrayAsync();
        }

        public async Task<T> GetItem<T>(string id) where T : class
        {
            return await scraperDbContext.FindAsync<T>(id);               
        }

        public async Task AddItem<T>(T entity, bool save = false) where T : class
        {
            await scraperDbContext.AddAsync<T>(entity);
            if (save)
                await scraperDbContext.SaveChangesAsync();
        }

        public async Task UpdateItem<T>(T entity, bool save = false) where T : class
        {
            scraperDbContext.Entry(entity).State = EntityState.Modified;
            if (save)
                await scraperDbContext.SaveChangesAsync();
        }

        public async Task DeleteItem<T>(T entity, bool save = false) where T : class
        {
            scraperDbContext.Entry(entity).State = EntityState.Deleted;
            if (save)
                await scraperDbContext.SaveChangesAsync();
        }

        public async Task<int> SaveChanges()
        {
           return await scraperDbContext.SaveChangesAsync();
        }
    }
}
