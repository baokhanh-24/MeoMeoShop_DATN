namespace MeoMeo.Contract.Commons;

public abstract class BasePaging
{
    public int TotalRecords { get; set; } = 0;
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; }= 10;
}