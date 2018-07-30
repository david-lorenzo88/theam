using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Theam.API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public void Add(T entity)
        {
            _unitOfWork.Context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            T existing = _unitOfWork.Context.Set<T>().Find(entity);
            if (existing != null) _unitOfWork.Context.Set<T>().Remove(existing);
        }

        public Task<T[]> Get()
        {
            return _unitOfWork.Context.Set<T>().ToArrayAsync<T>();
        }

        public Task<T[]> Get(Expression<Func<T, bool>> predicate)
        {
            return _unitOfWork.Context.Set<T>().Where(predicate).ToArrayAsync<T>();
        }

        public Task SaveAsync()
        {
            return _unitOfWork.Commit();
        }

        public void Update(T entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Modified;
            _unitOfWork.Context.Set<T>().Attach(entity);
        }
    }
}
