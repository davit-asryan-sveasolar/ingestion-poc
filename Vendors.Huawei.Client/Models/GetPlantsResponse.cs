namespace Vendors.Huawei.Client.Models;

public class GetPlantsResponse : BaseResponse
{
    public GetPlantsResponse(bool success, int failCode, string? message, PagedData<Plant>? data) : base(success, failCode, message)
    {
        Data = data;
    }

    public PagedData<Plant>? Data { get; }
}