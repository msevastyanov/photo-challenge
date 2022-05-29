using PhotoChallenge.Domain.Enums;

namespace PhotoChallenge.Domain.Exceptions
{
    public  class AppException : Exception
    {
        public ErrorCode Code { get; set; }

        public AppException(ErrorCode code, string message) : base(message)
        {
            Code = code;
        }
    }
}
