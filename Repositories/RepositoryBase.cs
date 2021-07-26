using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Backend.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected MyContext Context { get; set; }

        public RepositoryBase(MyContext context)
        {
            this.Context = context;
        }

        public IQueryable<T> FindAll(bool tracking)
        {
            if (tracking)
            {
                return this.Context.Set<T>();
            }
            else
            {
                return this.Context.Set<T>().AsNoTracking();
            }
        }

        public IQueryable<T> FindByCondition(
            Expression<Func<T, bool>> expression,
            bool tracking)
        {
            if (tracking)
            {
                return this.Context.Set<T>().Where(expression);
            }
            else
            {
                return this.Context.Set<T>().Where(expression).AsNoTracking();
            }
        }

        public T Single(Expression<Func<T, bool>> expression)
        {
            return this.Context.Set<T>().SingleOrDefault(expression);
        }       

        public void Create(T entity)
        {
            this.Context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            this.Context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            this.Context.Set<T>().Remove(entity);
        }

        public void DeleteAll()
        {
            var all = this.Context.Set<T>();
            this.Context.Set<T>().RemoveRange(all);
        }

        public IIncludableQueryable<T, TProperty> Include<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return this.Context.Set<T>().Include(expression);
        }     

        public EntityEntry Entry(T entity){
            return this.Context.Entry(entity);
        }
    }
}