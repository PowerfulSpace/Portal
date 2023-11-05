using Microsoft.EntityFrameworkCore;
using PS.Portal.DAL.Data;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.DAL.Repositories
{
    public class MovieRepository : IMovie
    {
        private readonly ApplicationContext _context;

        public MovieRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Movie>> GetItemsAsync(string sortProperty, SortOrder order, string searchText, int pageIndex, int pageSize)
        {
            List<Movie> items;

            if (searchText != "" && searchText != null)
            {
                items = _context.Movies
                    .Where(x => x.Name.Contains(searchText) || x.Description.Contains(searchText))
                    .ToList();
            }
            else
            {
                items = _context.Movies.ToList();
            }

            items = await DoSortAsync(items, sortProperty, order);

            PaginatedList<Movie> retUnits = new PaginatedList<Movie>(items, pageIndex, pageSize);

            return retUnits;
        }

        public async Task<Movie> GetItemAsync(Guid id)
        {
            return await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Movie> GreateAsync(Movie item)
        {
            await _context.Movies.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Movie> EditAsync(Movie item)
        {
            _context.Movies.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Movie> DeleteAsync(Movie item)
        {
            _context.Movies.Attach(item);
            _context.Entry(item).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return item;
        }


        private async Task<List<Movie>> DoSortAsync(List<Movie> items, string sortProperty, SortOrder order)
        {
            if (sortProperty.ToLower() == "name")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Name).ToList();
                else
                    items = items.OrderByDescending(x => x.Name).ToList();
            }
            else if (sortProperty.ToLower() == "description")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Description).ToList();
                else
                    items = items.OrderByDescending(x => x.Description).ToList();
            }
            else if (sortProperty.ToLower() == "rating")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Rating).ToList();
                else
                    items = items.OrderByDescending(x => x.Rating).ToList();
            }
            return items;
        }
    }
}
