using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Text;
using System.Linq;

namespace DAL.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        ValueTask<TEntity> GetByIdAsync(int id);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> CreateAsync(TEntity item);
        void Update(TEntity item);
        void Delete(int id);
    }
}
