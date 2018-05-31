namespace Prode.API.Models.Enums
{
    public enum CreateUserResult
    {
        CreatedOk = 1,
        BadParameters = 2,
        ErrorOnDatabase = 3,
        UserAlreadyExist = 4,
        MailAlreadyExist = 5
    }
}
