﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Common;

namespace Dsw2025Tpi.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetById(Guid id, params string[] include);
    Task<IEnumerable<T>?> GetAll(params string[] include);
    Task<T?> First(Expression<Func<T, bool>> predicate, params string[] include);
    Task<IEnumerable<T>?> GetFiltered(Expression<Func<T, bool>> predicate, params string[] include);
    Task<T> Add(T entity);
    Task<T> Update(T entity);
    Task<T> Delete(T entity);
}
