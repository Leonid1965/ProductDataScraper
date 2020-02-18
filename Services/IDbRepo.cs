using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductDataScraper.Services
{
    public interface IDbRepo 
    {
        Task AddRange<T>(T[] entities, bool save = false) where T : class;
        Task<int> SaveChanges();
        Task<T> GetItem<T>(string id) where T : class;
        Task<T[]> GetAll<T>() where T : class;
        Task<bool> DataExists<T>() where T : class;
        Task AddItem<T>(T entity, bool save = false) where T : class;
        Task UpdateItem<T>(T entity, bool save = false) where T : class;
        Task DeleteItem<T>(T entity, bool save = false) where T : class;
    }


}
