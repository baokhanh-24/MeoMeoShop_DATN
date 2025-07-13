namespace MeoMeo.Domain.Commons;

public static class PagingExtensions
{
    public class PagedResult<T>
    {
        public int TotalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<T> Items { get; set; }
        public PagedResult()
        {
            Items = new List<T>();
        }
    }
    public class PagedResult<T, TMeta> : PagedResult<T>
    {
        public TMeta? Metadata { get; set; }
    }
}