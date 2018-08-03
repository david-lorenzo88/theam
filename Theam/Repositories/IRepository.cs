using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Theam.API.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T[]> Get();
        Task<T[]> Get(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(object entityId);
        void Update(T entity);
        Task SaveAsync();
    }
}
