namespace RinhaDeBackend.API.Models
{
    public class ServiceResult<T>
    {
        public T Result { get; private set; }
        public Boolean IsError { get; private set; }
        public ServiceErrorCodeEnum ErrorCode { get; private set; }

        public ServiceResult(T result)
        {
            Result = result;
            IsError = false;
        }

        public ServiceResult(ServiceErrorCodeEnum errorCode)
        {
            ErrorCode = errorCode;
            IsError = true;
        }
    }
}
