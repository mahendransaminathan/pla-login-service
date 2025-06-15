using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1. Add CORS services and define a policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // or .WithOrigins("https://yourfrontend.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add services to the container
builder.Services.AddControllers(); // required for MapControllers
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=logins.db"));

// 🔐 Add JWT Authentication BEFORE calling builder.Build()
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSecret = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtSecret))
        throw new InvalidOperationException("JWT secret is not configured. Please set 'Jwt:Secret' in your configuration.");
    var key = Encoding.ASCII.GetBytes(jwtSecret);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});/*
// Add cookie + OpenID Connect for Azure AD SSO
.AddCookie("AdminCookie") // Cookie scheme for admin users after SSO login
.AddOpenIdConnect("AzureAD", options =>
{
    options.ClientId = builder.Configuration["AzureAd:ClientId"];
    options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
    options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/v2.0";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.SignInScheme = "AdminCookie";  // Uses cookie after successful login
    options.CallbackPath = "/signin-oidc";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email"); 
    options.SignedOutRedirectUri = "/";
});
*/
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();         // Or app.MapOpenApi();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll"); // <---- Apply CORS here!

app.UseAuthentication(); // 🔐 Must come before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
