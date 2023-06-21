namespace Vendors.Huawei.Client.Models;

public abstract class BaseResponse
{
    protected BaseResponse(bool success, int failCode, string? message)
    {
        Success = success;
        FailCode = failCode;
        Message = message;
    }

    public bool Success { get; }
    public int FailCode { get; }
    public string? Message { get; }
}