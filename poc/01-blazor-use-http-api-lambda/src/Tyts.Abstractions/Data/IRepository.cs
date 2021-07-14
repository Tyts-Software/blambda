
using System.Collections.Generic;
using System.Threading.Tasks;
using Tyts.Abstractions.Data.Search;

namespace Tyts.Abstractions.Data
{
    public interface IRepository<T, TId> where T : IEntity<TId>
    {
        T GetSingle(TId id);
        //IQueryable<T> GetAll();
        SearchResult<T> Find(ISearchParam param);

        //void Insert(T item);
        //void Update(T item);
        void Save(T item);
        void Delete(TId id);
    }

    public interface IRepositoryAsync<T> where T : IEntity
    {
        Task<T> GetSingleAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<SearchResult<T>> FindAsync(ISearchParam param);

        //void Insert(T item);
        //void Update(T item);
        Task SaveAsync(T item);
        Task DeleteAsync(object id);
    }

    public interface IRepositoryAsync<T, TId> where T : IEntity<TId>
    {
        Task<T> GetSingleAsync(TId id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<SearchResult<T>> FindAsync(ISearchParam param);

        //void Insert(T item);
        //void Update(T item);
        Task SaveAsync(T item);
        Task DeleteAsync(TId id);
    }
}