using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dirc.Domain.Interfaces
{
    public interface IRepository<T>
    {
        Task<Guid> InsertAsync(T entity);
        Task UpdateAsync(Guid id, T entity);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
    }
}
