using Microsoft.EntityFrameworkCore;
using PS.Portal.DAL.Data;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.DAL.Repositories
{
    public class ReviewRepository : IReview
    {
        private readonly ApplicationContext _context;

        public ReviewRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Review>> GetItemsAsync(string sortProperty, SortOrder order, string searchText, int pageIndex, int pageSize)
        {
            List<Review> items;

            if (searchText != "" && searchText != null)
            {
                items = _context.Reviews
                    .Include(x => x.Movie)
                        .Where(x => 
                        x.Login.Contains(searchText) ||
                        x.Text.Contains(searchText) ||
                        x.Movie!.Name.Contains(searchText))
                    .ToList();
            }
            else
            {
                items = _context.Reviews
                    .Include(x => x.Movie)
                    .ToList();
            }

            items = await DoSortAsync(items, sortProperty, order);

            PaginatedList<Review> retUnits = new PaginatedList<Review>(items, pageIndex, pageSize);

            return retUnits;
        }

        public async Task<Review> GetItemAsync(Guid id)
        {
            var item = await _context.Reviews.Include(x => x.Movie).FirstOrDefaultAsync(x => x.Id == id);

            return item;
        }

        public async Task<Review> GetItem_NoDownload_FG_Async(Guid id) => _context.Reviews.FirstOrDefault(x => x.Id == id);


        public async Task<Review> GreateAsync(Review item)
        {
            await _context.Reviews.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Review> EditAsync(Review item)
        {
            _context.Reviews.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Review> DeleteAsync(Review item)
        {
            item = await GetItem_NoDownload_FG_Async(item.Id);

            _context.Reviews.Attach(item);
            _context.Entry(item).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return item;
        }



        private async Task<List<Review>> DoSortAsync(List<Review> items, string sortProperty, SortOrder order)
        {
            if (sortProperty.ToLower() == "login")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Login).ToList();
                else
                    items = items.OrderByDescending(x => x.Login).ToList();
            }
            else if (sortProperty.ToLower() == "text")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Text).ToList();
                else
                    items = items.OrderByDescending(x => x.Text).ToList();
            }
            else if (sortProperty.ToLower() == "movie")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Movie?.Name).ToList();
                else
                    items = items.OrderByDescending(x => x.Movie?.Name).ToList();
            }
            return items;
        }
    }
}
