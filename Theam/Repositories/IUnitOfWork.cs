using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theam.API.Data;

namespace Theam.API.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        ApiContext Context { get; }
        Task Commit();
    }
}
