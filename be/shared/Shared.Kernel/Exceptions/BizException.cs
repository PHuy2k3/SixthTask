namespace Shared.Kernel.Exceptions;

public class BizException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public BizException(string code, string message, int statusCode = 400)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public BizException(string message, int statusCode = 400)
        : base(message)
    {
        Code = "BIZ_ERROR";
        StatusCode = statusCode;
    }
}