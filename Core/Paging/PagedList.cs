using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Core.Paging
{
    public class PagedList<TEntity>
    {
        public int TotalCount { get; set; }
        public IList<TEntity> Data { get; set; }

        public PagedList(List<TEntity> items, int count)
        {
            TotalCount = count;
            Data = items;
        }

        public static async Task<PagedList<TEntity>> CreateAsync(IQueryable<TEntity> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<TEntity>(items, count);
        }

        public static PagedList<TEntity> CreateAsync(IEnumerable<TEntity> source, int pageNumber, int pageSize, string searchKey = null)
        {
            var entities = source.ToList();
            var count = entities.Count;

            if (string.IsNullOrWhiteSpace(searchKey))
            {
                var items = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<TEntity>(items, count);
            }
            else
            {
                var items = entities.Where(m => m.GetType().GetProperties().Any(x =>
                        x.GetValue(m, null) != null && x.GetValue(m, null).ToString().ToLower().Contains(searchKey.ToLower())))
                    .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                return new PagedList<TEntity>(items, count);
            }

        }
    }
}
