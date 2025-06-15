public class LoginResponse
{
    public string Token { get; set; }
    public string Username { get; set; }
    
    public string Role { get; set; }          // user role, if you include it in JWT

    public LoginResponse(string token, string username, DateTime expiration, string role)
    {
        Token = token;
        Username = username;        
        Role = role;
    }
}