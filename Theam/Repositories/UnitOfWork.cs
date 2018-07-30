using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theam.API.Data;

namespace Theam.API.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public ApiContext Context { get; }

        public UnitOfWork(ApiContext context)
        {
            Context = context;
        }
        public Task Commit()
        {
            return Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();

        }
    }
}
