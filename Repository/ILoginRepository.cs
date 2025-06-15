

public interface ILoginRepository
{
    Task<Login> GetLogin(string username);
    Task<bool> ValidateLogin(string username, string password);
    Task<bool> CreateLogin(Login login);
    Task<bool> UpdateLogin(Login login);
    Task<bool> DeleteLogin(string username);
    Login Authenticate(Login login);

    public void Register(Login login);
}