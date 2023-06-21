namespace Vendors.Huawei.Client.Models;

public class LoginResponse : BaseResponse
{
    public LoginResponse(bool success, int failCode, string? message) : base(success, failCode, message)
    {
    }
}