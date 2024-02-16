namespace RinhaDeBackend.API.Models
{
    public enum ServiceErrorCodeEnum
    {
        None = 0,
        GenericFailure = 1,
        NotFound = 2,
        InsufficientLimit = 3,
        AlreadyLocked = 4,
    }
}