using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Interview_Test.Infrastructure.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Entities { get; }

        #region Create
        TEntity Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        Task<TEntity> AddEntityAsync(TEntity entity, CancellationToken cancellationToken = default);
        #endregion

        #region Read
        ValueTask<TEntity?> GetByIDAsync<TId>(TId id, CancellationToken cancellationToken = default);
        ValueTask<TEntity?> GetByIDAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> FindAsync(CancellationToken cancellationToken = default);
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        IQueryable<TEntity> QueryAll(Expression<Func<TEntity, bool>>? expression = null);
        Task<IQueryable<TEntity>> QueryAllAsync(Expression<Func<TEntity, bool>>? expression = null, CancellationToken cancellationToken = default);
        IEnumerable<TEntity> Inquiry(Expression<Func<TEntity, bool>>? filter = null,
                                     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                     string includeProperties = "");
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
        #endregion

        #region Update
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        #endregion

        #region Delete
        void Remove<TId>(TId id);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        #endregion

        
        #region Misc
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        #endregion
    }
}
