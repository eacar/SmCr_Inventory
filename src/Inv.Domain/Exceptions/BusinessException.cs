namespace Inv.Domain.Exceptions
{
    public class BusinessException : ExceptionBase
    {
        public BusinessException(string errorCode, string errorMessage)
            : base(errorCode, errorMessage)
        {
        }
    }
}