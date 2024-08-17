namespace ToDoListApp.Server.Authentication.Services.PasswordHasher
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string userPasswordHash);
    }
}
