namespace Vendors.Huawei.Client.Models;

public class GetRealTimeKpiResponse : BaseResponse
{
    public GetRealTimeKpiResponse(bool success, int failCode, string? message, GetRealTimeKpiResponseInner[]? data) : base(success, failCode, message)
    {
        Data = data;
    }
    
    public GetRealTimeKpiResponseInner[]? Data { get; }
}