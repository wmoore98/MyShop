using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using MyShop.Core.Models;
using MyShop.Core.Contracts;

namespace MyShop.DataAccess.InMemory
{
    public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
    {

        ObjectCache cache = MemoryCache.Default;
        List<T> items;
        string className;

        public InMemoryRepository()
        {
            className = typeof(T).Name;
            items = cache[className] as List<T>;
            if (items == null)
            {
                items = new List<T>();
            }
        }

        public void Commit()
        {
            cache[className] = items;
        }

        public void Insert(T item)
        {
            items.Add(item);
        }

        public void Update(T item)
        {
            T itemToUpdate = items.Find(i => i.Id == item.Id);

            if (itemToUpdate != null)
            {
                itemToUpdate = item;
            }
            else
            {
                throw new Exception($"{className} not found");
            }
        }

        public T Find(string Id)
        {
            T itemFound = items.Find(i => i.Id == Id);

            if (itemFound != null)
            {
                return itemFound;
            }
            else
            {
                throw new Exception($"{className} not found");
            }
        }

        public IQueryable<T> Collection()
        {
            return items.AsQueryable();
        }

        public void Delete(string Id)
        {
            T itemToDelete = items.Find(i => i.Id == Id);

            if (itemToDelete != null)
            {
                items.Remove(itemToDelete);
            }
            else
            {
                throw new Exception($"{className} not found");
            }
        }
    }
}
