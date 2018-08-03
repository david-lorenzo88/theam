using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Theam.API.Repositories
{
    /// <summary>
    /// Generic repository class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly IUnitOfWork _unitOfWork;
        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Adds an entity to the context
        /// </summary>
        /// <param name="entity">the entity</param>
        public void Add(T entity)
        {
            _unitOfWork.Context.Set<T>().Add(entity);
        }
        /// <summary>
        /// Deletes an entity from the context
        /// </summary>
        /// <param name="entity">the entity</param>
        public void Delete(object entityId)
        {
            T existing = _unitOfWork.Context.Set<T>().Find(entityId);
            if (existing != null) _unitOfWork.Context.Set<T>().Remove(existing);
        }
        /// <summary>
        /// Gets all entities from the context
        /// </summary>
        /// <returns></returns>
        public Task<T[]> Get()
        {
            return _unitOfWork.Context.Set<T>().ToArrayAsync<T>();
        }
        /// <summary>
        /// Gets filtered entities from the context
        /// </summary>
        /// <param name="predicate">the filter predicate</param>
        /// <returns></returns>
        public Task<T[]> Get(Expression<Func<T, bool>> predicate)
        {
            return _unitOfWork.Context.Set<T>().Where(predicate).ToArrayAsync<T>();
        }
        /// <summary>
        /// Commits the changes from the context to the database
        /// </summary>
        /// <returns></returns>
        public Task SaveAsync()
        {
            return _unitOfWork.Commit();
        }
        /// <summary>
        /// Updates an entity
        /// </summary>
        /// <param name="entity">the entity</param>
        public void Update(T entity)
        {
            _unitOfWork.Context.Entry(entity).State = EntityState.Modified;
            _unitOfWork.Context.Set<T>().Attach(entity);
        }
    }
}
