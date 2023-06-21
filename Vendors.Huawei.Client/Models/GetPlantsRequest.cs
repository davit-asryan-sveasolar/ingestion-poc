namespace Vendors.Huawei.Client.Models;

public class GetPlantsRequest
{
    public GetPlantsRequest(int pageNo, int pageSize)
    {
        PageNo = pageNo;
        PageSize = pageSize;
    }

    public int PageNo { get; }
    public int PageSize { get; }
}