namespace Inv.Domain.Exceptions
{
    public class ExceptionBase : Exception
    {
        public string ErrorCode { get; set; }

        public ExceptionBase(string errorCode, string message)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}