
public class LoginRepository : ILoginRepository
{
    private readonly List<Login> _logins = new List<Login>();

    private readonly AppDbContext _context;
    
    public LoginRepository(AppDbContext context)
    {
        _context = context;        
    }
    public Task<Login> GetLogin(string username)
    {
        var login = _logins.FirstOrDefault(l => l.Username == username);
        return Task.FromResult(login);
    }

    public Task<bool> ValidateLogin(string username, string password)
    {
        var login = _logins.FirstOrDefault(l => l.Username == username && l.PasswordHash == password);
        return Task.FromResult(login != null);
    }

    public Task<bool> CreateLogin(Login login)
    {
        if (_logins.Any(l => l.Username == login.Username))
            return Task.FromResult(false);

        _logins.Add(login);
        return Task.FromResult(true);
    }

    public Task<bool> UpdateLogin(Login login)
    {
        var existingLogin = _logins.FirstOrDefault(l => l.Username == login.Username);
        if (existingLogin == null)
            return Task.FromResult(false);

        existingLogin.PasswordHash = login.PasswordHash;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteLogin(string username)
    {
        var login = _logins.FirstOrDefault(l => l.Username == username);
        if (login == null)
            return Task.FromResult(false);

        _logins.Remove(login);
        return Task.FromResult(true);
    }

    public Login Authenticate(Login login)
    {
        var user = _context.Logins.SingleOrDefault(u => u.Username == login.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
            return null;

        return user;
    }
    
    public void Register(Login login)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(login.PasswordHash);
        login.PasswordHash = hashedPassword;
        
        _context.Logins.Add(login);
        _context.SaveChanges();
    }
}