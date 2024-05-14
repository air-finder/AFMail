using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data.Repository
{
    [ExcludeFromCodeCoverage]
    public class BaseRepository<TEntity>(IUnitOfWork unitOfWork) : IBaseRepository<TEntity> where TEntity : class
    {
        public IQueryable<TEntity> GetAll() => unitOfWork.Context.Set<TEntity>().AsQueryable();

        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> expression) => await GetAll().Where(expression).ToListAsync();
        public async Task<IEnumerable<TEntity>> GetNoTrackingAsync(Expression<Func<TEntity, bool>> expression) => await GetAll().AsNoTracking().Where(expression).ToListAsync();

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> expression) => GetAll().Where(expression);

        public virtual async Task<TEntity?> GetByIDAsync(Guid id)
        {
            return await unitOfWork.Context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task InsertWithSaveChangesAsync(TEntity entity)
        {
            await unitOfWork.Context.Set<TEntity>().AddAsync(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await unitOfWork.Context.Set<TEntity>().AddAsync(entity);
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            TEntity? entityToDelete = await unitOfWork.Context.Set<TEntity>().FindAsync(id);
            await DeleteAsync(entityToDelete!);
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            if (unitOfWork.Context.Entry(entity).State == EntityState.Detached)
            {
                unitOfWork.Context.Set<TEntity>().Attach(entity);
            }
            unitOfWork.Context.Set<TEntity>().Remove(entity);
            await unitOfWork.SaveChangesAsync();
        }

        public virtual async Task UpdateWithSaveChangesAsync(TEntity entity)
        {
            unitOfWork.Context.Set<TEntity>().Attach(entity);
            unitOfWork.Context.Entry(entity).State = EntityState.Modified;
            await unitOfWork.SaveChangesAsync();
        }

        public virtual void Update(TEntity entity)
        {
            unitOfWork.Context.Set<TEntity>().Attach(entity);
            unitOfWork.Context.Entry(entity).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await GetAll().AnyAsync(expression);
        }
    }
}
