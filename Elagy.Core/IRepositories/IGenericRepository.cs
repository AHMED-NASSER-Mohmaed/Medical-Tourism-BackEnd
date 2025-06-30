using System;
using System.Collections.Generic;
using System.Linq; // Added for IQueryable
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<T> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);




        // overloads for including related entities
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);



        Task<T> GetByIdAsync(string id, params Expression<Func<T, object>>[] includeProperties);



        // This method is crucial for fluent LINQ queries with EF Core, including .Include()
        //we are going to remove this function in the future becouse it expose the details of the ORM
        IQueryable<T> AsQueryable();
    }
}