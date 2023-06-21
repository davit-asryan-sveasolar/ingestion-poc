namespace Vendors.Huawei.Client.Models;

public class PagedData<T>
{
    public PagedData(T[] list, int pageCount, int pageNo, int pageSize, int total)
    {
        List = list;
        PageCount = pageCount;
        PageNo = pageNo;
        PageSize = pageSize;
        Total = total;
    }

    public T[] List { get; }
    public int PageCount { get; }
    public int PageNo { get; }
    public int PageSize { get; }
    public int Total { get; }
}