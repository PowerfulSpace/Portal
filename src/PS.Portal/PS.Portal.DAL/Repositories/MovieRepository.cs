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
                .Where(x =>
                x.Name.Contains(searchText) ||
                x.Description.Contains(searchText) ||
                x.Country!.Name.Contains(searchText) ||
                x.CurrentProducer!.FirstName.Contains(searchText) ||
                x.CurrentProducer!.LastName.Contains(searchText))
                    .Include(x => x.Country)
                    .Include(x => x.CurrentProducer)
                    .Include(x => x.Actors)
                    .Include(x => x.Genres)
                    .Include(x => x.Reviews)
                    .ToList();
            }
            else
            {
                items = _context.Movies
                    .Include(x => x.Country)
                    .Include(x => x.CurrentProducer)
                    .Include(x => x.Actors)
                    .Include(x => x.Genres)
                    .Include(x => x.Reviews)
                        .ToList();
            }

            items = await DoSortAsync(items, sortProperty, order);

            PaginatedList<Movie> retUnits = new PaginatedList<Movie>(items, pageIndex, pageSize);

            return retUnits;
        }

        public async Task<Movie> GetItemAsync(Guid id)
        {
            var item = await _context.Movies
                .Include(x => x.Country)
                .Include(x => x.CurrentProducer)
                .Include(x => x.Actors)
                .Include(x => x.Genres)
                .Include(x => x.Reviews)
                    .FirstOrDefaultAsync(x => x.Id == id);

            item!.BreifPhotoName = GetBriefPhotoName(item.PhotoUrl);

            return item;
        }

        public async Task<Movie> GetItem_NoDownload_FG_Async(Guid id) => _context.Movies.FirstOrDefault(x => x.Id == id);


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
            item = await GetItem_NoDownload_FG_Async(item.Id);

            _context.Movies.Attach(item);
            _context.Entry(item).State = EntityState.Deleted;
            await _context.SaveChangesAsync();
            return item;
        }


        public bool IsItemNameExists(string name)
        {
            int ct = _context.Movies.Where(x => x.Name.ToLower() == name.ToLower()).Count();
            if (ct > 0)
                return true;
            else
                return false;
        }
        public bool IsItemNameExists(string name, Guid id)
        {
            int ct = _context.Movies.Where(x => x.Name.ToLower() == name.ToLower() && x.Id != id).Count();
            if (ct > 0)
                return true;
            else
                return false;
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
            else if (sortProperty.ToLower() == "yearShown")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.ReleaseYear).ToList();
                else
                    items = items.OrderByDescending(x => x.ReleaseYear).ToList();
            }
            else if (sortProperty.ToLower() == "filmDuration")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.FilmDuration).ToList();
                else
                    items = items.OrderByDescending(x => x.FilmDuration).ToList();
            }
            else if (sortProperty.ToLower() == "acceptableAge")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.AcceptableAge).ToList();
                else
                    items = items.OrderByDescending(x => x.AcceptableAge).ToList();
            }
            else if (sortProperty.ToLower() == "isReaded")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.IsReaded).ToList();
                else
                    items = items.OrderByDescending(x => x.IsReaded).ToList();
            }
            else if (sortProperty.ToLower() == "partFilm")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.PartFilm).ToList();
                else
                    items = items.OrderByDescending(x => x.PartFilm).ToList();
            }
            else if (sortProperty.ToLower() == "country")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.Country?.Name).ToList();
                else
                    items = items.OrderByDescending(x => x.Country?.Name).ToList();
            }
            else if (sortProperty.ToLower() == "producer")
            {
                if (order == SortOrder.Ascending)
                    items = items.OrderBy(x => x.CurrentProducer?.LastName).ToList();
                else
                    items = items.OrderByDescending(x => x.CurrentProducer?.LastName).ToList();
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
