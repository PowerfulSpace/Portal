using PS.Portal.DAL.Interfaces.Base;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Interfaces
{
    public interface IActor : IBaseRepository<Actor>
    {
        public bool IsItemNameExists(string firstName, string lastName);
        public bool IsItemNameExists(string firstName, string lastName, Guid id);
    }
}
