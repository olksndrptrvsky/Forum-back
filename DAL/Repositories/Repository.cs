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

        public TEntity CreateAsync(TEntity item)
        {
            _dbSet.AddAsync(item);
            return item;
        }

        public void Delete(int id)
        {
            TEntity item = _dbSet.Find(id);
            if (item != null)
            {
                _db.Remove(item);
            }
        }

        public ValueTask<TEntity> GetByIdAsync(int id)
        {
            return _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public void Update(TEntity item)
        {
            _db.Update(item);
        }
    }
}
