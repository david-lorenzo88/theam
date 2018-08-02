using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theam.API.Data;

namespace Theam.API.Repositories
{
    /// <summary>
    /// Unit of work class to manage transaction with the database
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        public ApiContext Context { get; }

        public UnitOfWork(ApiContext context)
        {
            Context = context;
        }
        /// <summary>
        /// Saves the changes to database
        /// </summary>
        /// <returns></returns>
        public Task Commit()
        {
            return Context.SaveChangesAsync();
        }
        /// <summary>
        /// Free up resources
        /// </summary>
        public void Dispose()
        {
            Context.Dispose();

        }
    }
}
