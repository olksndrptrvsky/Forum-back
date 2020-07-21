using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using DAL.Interfaces;
using DAL.EF;

namespace DAL.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: class
    {
        private ForumContext _db;
        private DbSet<TEntity> _dbSet;

        public Repository(ForumContext context)
        {
            _db = context;
            _dbSet = _db.Set<TEntity>();
        }

        public async Task<TEntity> CreateAsync(TEntity item)
        {
            var result = await _dbSet.AddAsync(item);
           
            return result.Entity;
        }

        public void Delete(params object[] keyValues)
        {
            TEntity item = _dbSet.Find(keyValues);
            if (item != null)
            {
                _db.Remove(item);
            }
        }

        public ValueTask<TEntity> GetByIdAsync(params object[] keyValues)
        {
            return _dbSet.FindAsync(keyValues);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate);
        }

        public void Update(TEntity item)
        {
            _db.Update(item);
        }
    }
}
