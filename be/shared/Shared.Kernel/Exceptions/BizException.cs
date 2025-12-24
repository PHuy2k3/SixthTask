namespace Shared.Kernel;

public class BizException : Exception
{
    public int StatusCode { get; }

    public BizException(string message, int statusCode = 400)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
