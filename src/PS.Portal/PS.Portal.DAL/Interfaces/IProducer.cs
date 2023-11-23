using PS.Portal.DAL.Interfaces.Base;
using PS.Portal.Domain.Entities;

namespace PS.Portal.DAL.Interfaces
{
    public interface IProducer : IBaseRepository<Producer>
    {
        public bool IsItemNameExists(string firstName, string lastName);
        public bool IsItemNameExists(string firstName, string lastName, Guid id);
    }
}
