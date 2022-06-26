using Microsoft.EntityFrameworkCore;

namespace Application.Common.Helpers
{
    public class PagedList<T> : List<T> where T : class
    {
        #region Constructor

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize); // divide total items with page size
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        #endregion

        #region Properties
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        /// <summary>
        /// it could be all users or users according to the query
        /// eg list of females
        /// </summary>
        public int TotalCount { get; set; }

        #endregion

        #region Method

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync();

            // eg suppose pageNumber = 2 and pageSize = 5 and total data is 15, so Skip((2-1)*5) => skip(5)
            // Skip(5).Take(5) from 15 IQuerayble data
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);    
        }

        #endregion

    }
}
