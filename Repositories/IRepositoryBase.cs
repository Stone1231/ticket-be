using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;

namespace Backend.Repositories
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool tracking = false);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool tracking = false);
        T Single(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteAll();
        IIncludableQueryable<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> expression);
        EntityEntry Entry(T entity);
    }
}