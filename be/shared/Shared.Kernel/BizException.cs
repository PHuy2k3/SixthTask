namespace Shared.Kernel;

public sealed class BizException : Exception
{
    public string Code { get; }
    public int HttpStatus { get; }

    public BizException(string code, string message, int httpStatus = 400) : base(message)
    {
        Code = code;
        HttpStatus = httpStatus;
    }
}
