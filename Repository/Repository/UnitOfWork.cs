using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadLater.Data;
using ReadLater.Entities;

namespace ReadLater.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDbContext _context;

        private bool _disposed;
        private Hashtable _repositories;

        public string CurrentUserId;

        public UnitOfWork(IDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            foreach (var auditableEntity in _context.ChangeTracker.Entries<IMustHaveUser>())
            {
                auditableEntity.Entity.UserId = CurrentUserId;
            }

            _context.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }

        public void SetCurrentUserId(string id)
        {
            CurrentUserId = id;
        }
    }
}
