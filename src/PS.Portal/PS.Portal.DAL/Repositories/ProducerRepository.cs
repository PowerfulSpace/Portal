using Microsoft.EntityFrameworkCore;
using PS.Portal.DAL.Data;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.DAL.Repositories
{
    public class ProducerRepository : IProducer
    {
        private readonly ApplicationContext _context;

        public ProducerRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Producer>> GetItemsAsync(string sortProperty, SortOrder order, string searchText, int pageIndex, int pageSize)
        {
            List<Producer> items;

            if (searchText != "" && searchText != null)
            {
                items = _context.Producers
                    .Include(x => x.Country)
                    .Include(x => x.Movies)
                        .Where(x => 
                        x.FirstName.Contains(searchText) ||
                        x.LastName.Contains(searchText) ||
                        x.Country!.Name.Contains(searchText))
                    .ToList();
            }
            else
            {
                items = _context.Producers
                    .Include(x => x.Country)
                    .Include(x => x.Movies)
                        .ToList();
            }

            items = await DoSortAsync(items, sortProperty, order);

            PaginatedList<Producer> retUnits = new PaginatedList<Producer>(items, pageIndex, pageSize);

            return retUnits;
        }

        public async Task<Producer> GetItemAsync(Guid id)
        {
            var item = await _context.Producers
                 .Include(x => x.Country)
                 .Include(x => x.Movies)
                .FirstOrDefaultAsync(x => x.Id == id);

            item!.BreifPhotoName = GetBriefPhotoName(item.PhotoUrl);

            return item;
        }

        public async Task<Producer> GetItem_NoDownload_FG_Async(Guid id) => _context.Producers.FirstOrDefault(x => x.Id == id);


        public async Task<Producer> GreateAsync(Producer item)
        {
            await _context.Producers.AddAsync(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Producer> EditAsync(Producer item)
        {
            _context.Producers.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Producer> DeleteAsync(Producer item)
        {
            item = await GetItem_NoDownload_FG_Async(item.Id);

            _context.Producers.Attach(item);
            _context.Entry(item).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return item;
        }


        public bool IsItemNameExists(string firstName, string lastName)
        {
            int ct = _context.Producers
                 .Where(x => x.FirstName.ToLower() == firstName.ToLower() && x.LastName.ToLower() == lastName.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }
        public bool IsItemNameExists(string firstName, string lastName, Guid id)
        {
            int ct = _context.Producers
                .Where(x => x.FirstName.ToLower() == firstName.ToLower() && x.LastName.ToLower() == lastName.ToLower() && x.Id != id).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }



        private async Task<List<Producer>> DoSortAsync(List<Producer> items, string sortProperty, SortOrder order)
        {
            if (sortProperty.ToLower() == "firstName")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.FirstName).ToList();
                else
                    items = items.OrderByDescending(x => x.FirstName).ToList();
            }
            else if (sortProperty.ToLower() == "lastName")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.LastName).ToList();
                else
                    items = items.OrderByDescending(x => x.LastName).ToList();
            }
            else if (sortProperty.ToLower() == "birthDate")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.BirthDate).ToList();
                else
                    items = items.OrderByDescending(x => x.BirthDate).ToList();
            }
            else if (sortProperty.ToLower() == "country")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Country?.Name).ToList();
                else
                    items = items.OrderByDescending(x => x.Country?.Name).ToList();
            }
            return items;
        }

        private string GetBriefPhotoName(string fileName)
        {
            if (fileName == null || fileName == "")
                return "";

            string[] words = fileName.Split('_');
            return words[words.Length - 1];
        }
    }
}
