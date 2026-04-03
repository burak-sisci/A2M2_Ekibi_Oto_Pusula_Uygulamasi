namespace backend.API.Modules.Auth.Application;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
