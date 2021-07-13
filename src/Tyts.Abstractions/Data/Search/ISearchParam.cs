namespace Tyts.Abstractions.Data.Search
{
    public interface ISearchParam
    {
        int Offset { get; set; }
        int PageSize { get; set; }
        string OrderBy { get; set; }
    }
}