using PS.Portal.DAL.Interfaces.Base;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Interfaces
{
    public interface IMovie : IBaseRepository<Movie>
    {
        public bool IsItemNameExists(string name);
        public bool IsItemNameExists(string name, Guid id);

    }
}
