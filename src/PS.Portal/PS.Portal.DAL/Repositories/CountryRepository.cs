using Microsoft.EntityFrameworkCore;
using PS.Portal.DAL.Data;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.DAL.Repositories
{
    public class CountryRepository : ICountry
    {
        private readonly ApplicationContext _context;

        public CountryRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Country>> GetItemsAsync(string sortProperty, SortOrder order, string searchText, int pageIndex, int pageSize)
        {
            List<Country> items;

            if (searchText != "" && searchText != null)
            {
                items = _context.Countries
                    .Include(x => x.Movies)
                    .Include(x => x.Actors)
                    .Include(x => x.Producers)
                        .Where(x => x.Name.Contains(searchText))
                    .ToList();
            }
            else
            {
                items = _context.Countries
                    .Include(x => x.Movies)
                    .Include(x => x.Actors)
                    .Include(x => x.Producers)
                        .ToList();
            }

            items = await DoSortAsync(items, sortProperty, order);

            PaginatedList<Country> retUnits = new PaginatedList<Country>(items, pageIndex, pageSize);

            return retUnits;
        }

        public async Task<Country> GetItemAsync(Guid id)
        {
            var item = await _context.Countries
                .Include(x => x.Movies)
                .Include(x => x.Actors)
                .Include(x => x.Producers)
                    .FirstOrDefaultAsync(x => x.Id == id);

            return item!;
        }

        public async Task<Country> GetItem_NoDownload_FG_Async(Guid id) => _context.Countries.FirstOrDefault(x => x.Id == id);


        public async Task<Country> GreateAsync(Country item)
        {
            await _context.Countries.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Country> EditAsync(Country item)
        {
            _context.Countries.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Country> DeleteAsync(Country item)
        {
            item = await GetItem_NoDownload_FG_Async(item.Id);

            _context.Countries.Attach(item);
            _context.Entry(item).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return item;
        }


        public bool IsItemNameExists(string name)
        {
            int ct = _context.Countries.Where(x => x.Name.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }
        public bool IsItemNameExists(string name, Guid id)
        {
            int ct = _context.Countries.Where(x => x.Name.ToLower() == name.ToLower() && x.Id != id).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }



        private async Task<List<Country>> DoSortAsync(List<Country> items, string sortProperty, SortOrder order)
        {
            if (sortProperty.ToLower() == "name")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Name).ToList();
                else
                    items = items.OrderByDescending(x => x.Name).ToList();
            }
            return items;
        }
    }
}
