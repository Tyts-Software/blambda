using System.Collections;
using System.Collections.Generic;

namespace Tyts.Abstractions.Data.Search
{

    public abstract class ASearchParam<TQuery> : ISearchParam
    {
        public int? Lcid { get; set; }
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public int PageIndex 
        { 
            get
            {
                return Offset > PageSize ? Offset / PageSize : 0;
            }    
        }

        public virtual string OrderBy { get; set; }

        protected ASearchParam()
        {
            PageSize = 20;
        }
            

        protected internal abstract TQuery AddWhereClause(TQuery criteria);
    }
    
    public class SearchResult<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _result;
        public int Total { get; private set; }

        public SearchResult(IEnumerable<T> result, int total)
        {
            _result = result;
            Total = total;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return _result.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class SearchParamExtention
    {
        public static ISearchParam Sort(this ISearchParam criteria, string orderBy)
        {
            criteria.OrderBy = orderBy;
            return criteria;
        }

        /// <summary>
        /// use 'def' if 'orderBy' IsNullOrEmpty
        /// </summary>
        public static ISearchParam Sort(this ISearchParam criteria, string orderBy, string def)
        {
            criteria.OrderBy = !string.IsNullOrEmpty(orderBy) ? def : orderBy;
            return criteria;
        }

        public static ISearchParam Skip(this ISearchParam criteria, int skip)
        {
            criteria.Offset = skip;
            return criteria;
        }

        public static ISearchParam Take(this ISearchParam criteria, int take)
        {
            criteria.PageSize = take;
            return criteria;
        }

    }
}