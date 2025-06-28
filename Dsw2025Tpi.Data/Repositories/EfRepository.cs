﻿using Dsw2025Tpi.Domain.Common;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Repositories;

public class EfRepository<T> : IRepository<T> where T : EntityBase
{
    private readonly Dsw2025TpiContext _context;

    public EfRepository(Dsw2025TpiContext context)
    {
        _context = context;
    }

    public async Task<T> Add(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> Update(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> Delete(T entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> GetById(Guid id, params string[] include)
    {
        return await Include(_context.Set<T>(), include)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<T?> First(Expression<Func<T, bool>> predicate, params string[] include)
    {
        return await Include(_context.Set<T>(), include)
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>?> GetAll(params string[] include)
    {
        return await Include(_context.Set<T>(), include)
            .ToListAsync();
    }

    public async Task<IEnumerable<T>?> GetFiltered(Expression<Func<T, bool>> predicate, params string[] include)
    {
        return await Include(_context.Set<T>(), include)
            .Where(predicate)
            .ToListAsync();
    }

    private static IQueryable<T> Include(IQueryable<T> query, string[] includes)
    {
        foreach (var include in includes)
            query = query.Include(include);
        return query;
    }
}
