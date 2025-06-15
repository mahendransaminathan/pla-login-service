
public interface ILoginService
{
    Login Authenticate(Login login);
    string GenerateJwtToken(Login login);

    void Register(Login login);
}
