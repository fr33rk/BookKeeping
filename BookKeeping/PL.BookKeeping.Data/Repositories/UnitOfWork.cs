using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using PL.BookKeeping.Entities;
using PL.BookKeeping.Infrastructure.Data;

namespace PL.BookKeeping.Data.Repositories
{
    /// <summary>
    /// Implementation of a EntityFramework-based unit of work.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext _dbContext;
        private Dictionary<Type, object> _repositories;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork" /> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dbContext" /> is <c>null</c>.</exception>
        public UnitOfWork(DbContext dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");

            this._dbContext = dbContext;
            this._repositories = new Dictionary<Type, object>();
        }

        /// <summary>Sets the current user int the DbContext.</summary>
        /// <param name="currentUser">The current user.</param>
        public void SetCurrentUser(User currentUser)
        {
            ((DataContext)this._dbContext).CurrentUser = currentUser;
        }

        /// <summary>
        /// Saves the changes inside the unit of work.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the context has been disposed.</exception>
        public void SaveChanges()
        {
            try
            {
                this._dbContext.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                //TODO-ML: Log these?
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Gets the repository for the specified type that is created specifically for this unit of work.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity for which to get the repository.</typeparam>
        /// <returns>The entity repository.</returns>
        /// <exception cref="NotSupportedException">A repository for the specified type cannot be found.</exception>
        public IEntityRepositoryOfT<TEntity> GetRepository<TEntity>() where TEntity : class, new()
        {
            IEntityRepositoryOfT<TEntity> repository = null;
            if (this._repositories.ContainsKey(typeof(TEntity)))
            {
                repository = this._repositories[typeof(TEntity)] as IEntityRepositoryOfT<TEntity>;
            }

            if (repository == null)
            {
                repository = new EntityRepositoryOfT<TEntity>(this._dbContext);

                this._repositories[typeof(TEntity)] = repository;
            }

            return repository;
        }

        #region IDisposable implementation

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destructs this instance and releases unmanaged and - optionally - managed resources.
        /// </summary>
        ~UnitOfWork()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (_dbContext != null)
                    {
                        _dbContext.Dispose();
                    }
                }

                this._disposed = true;
            }
        }

        #endregion
    }
}