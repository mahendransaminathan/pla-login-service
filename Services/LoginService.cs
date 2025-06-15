using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

public class LoginService : ILoginService
{
    
    private readonly ILoginRepository _loginRepository;
    
    private readonly IConfiguration _config;
    public LoginService(ILoginRepository loginRepository, IConfiguration config)
    {
        _loginRepository = loginRepository;
        _config = config;

    }
    

    public Login Authenticate(Login login)
    {
        var user = _loginRepository.Authenticate(login);
        if (user == null || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
            return null;

        return user;
    }

    public string GenerateJwtToken(Login user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT secret is not configured.");
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public void Register(Login login)
    {
        _loginRepository.Register(login);
    }
}
