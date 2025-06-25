using Dsw2025Tpi.Domain.Common;
using Dsw2025Tpi.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : EntityBase
    {
        protected readonly Dsw2025TpiContext _context;

        public EfRepository(Dsw2025TpiContext context)
        {
            _context = context;
        }

        public async Task<T?> GetById(Guid id, params string[] include)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var navigation in include)
                query = query.Include(navigation);

            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<T>?> GetAll(params string[] include)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var navigation in include)
                query = query.Include(navigation);

            var list = await query.ToListAsync();
            return list.Count > 0 ? list : null;
        }

        public async Task<T?> First(Expression<Func<T, bool>> predicate, params string[] include)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var navigation in include)
                query = query.Include(navigation);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>?> GetFiltered(Expression<Func<T, bool>> predicate, params string[] include)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var navigation in include)
                query = query.Include(navigation);

            var list = await query.Where(predicate).ToListAsync();
            return list.Count > 0 ? list : null;
        }

        public async Task<T> Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            // No llamo a SaveChangesAsync aquí: el UnitOfWork se encarga de confirmar
            return entity;
        }

        public async Task<T> Update(T entity)
        {
            _context.Set<T>().Update(entity);
            // No llamo a SaveChangesAsync aquí: el UnitOfWork se encarga de confirmar
            return await Task.FromResult(entity);
        }

        public async Task<T> Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            // No llamo a SaveChangesAsync aquí: el UnitOfWork se encarga de confirmar
            return await Task.FromResult(entity);
        }
    }
}
