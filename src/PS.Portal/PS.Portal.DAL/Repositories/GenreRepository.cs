using Microsoft.EntityFrameworkCore;
using PS.Portal.DAL.Data;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.DAL.Repositories
{
    public class GenreRepository : IGenre
    {
        private readonly ApplicationContext _context;

        public GenreRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Genre>> GetItemsAsync(string sortProperty, SortOrder order, string searchText, int pageIndex, int pageSize)
        {
            List<Genre> items;

            if (searchText != "" && searchText != null)
            {
                items = _context.Genres
                    .Include(x => x.Movies)
                    .Where(x => x.Name.Contains(searchText) || x.Description.Contains(searchText))
                    .ToList();
            }
            else
            {
                items = _context.Genres
                    .Include(x => x.Movies)
                    .ToList();
            }

            items = await DoSortAsync(items, sortProperty, order);

            PaginatedList<Genre> retUnits = new PaginatedList<Genre>(items, pageIndex, pageSize);

            return retUnits;
        }

        public async Task<Genre> GetItemAsync(Guid id)
        {
            var item = await _context.Genres.FirstOrDefaultAsync(x => x.Id == id);
            return item!;
        }
        public async Task<Genre> GetItem_NoDownload_FG_Async(Guid id) => _context.Genres.FirstOrDefault(x => x.Id == id);


        public async Task<Genre> GreateAsync(Genre item)
        {
            await _context.Genres.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Genre> EditAsync(Genre item)
        {
            _context.Genres.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Genre> DeleteAsync(Genre item)
        {
            item = await GetItem_NoDownload_FG_Async(item.Id);

            _context.Genres.Attach(item);
            _context.Entry(item).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return item;
        }


        public bool IsItemNameExists(string name)
        {
            int ct = _context.Genres.Where(x => x.Name.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }
        public bool IsItemNameExists(string name, Guid id)
        {
            int ct = _context.Genres.Where(x => x.Name.ToLower() == name.ToLower() && x.Id != id).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }



        private async Task<List<Genre>> DoSortAsync(List<Genre> items, string sortProperty, SortOrder order)
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
            return items;
        }
    }
}
