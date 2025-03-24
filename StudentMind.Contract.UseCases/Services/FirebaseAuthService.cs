using Microsoft.IdentityModel.Tokens;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using StudentMind.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StudentMind.Services.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public FirebaseAuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> SignInWithEmailPasswordAsync(string email, string password)
        {
            try
            {
                var apiKey = _configuration["Firebase:ApiKey"];
                var signInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(signInUrl, content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Invalid email or password.");

                var responseData = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
                var idToken = responseData.GetProperty("idToken").GetString();

                // Check if user exists
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.Entities.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Create a new user
                    user = new User
                    {
                        Username = email.Split('@')[0],
                        Email = email,
                        FullName = "Unknown",
                        RoleId = await GetDefaultRoleIdAsync()
                    };

                    await userRepo.InsertAsync(user);
                    await _unitOfWork.SaveAsync();
                }

                // Generate JWT Token
                return await GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Firebase authentication failed: {ex.Message}");
            }
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? throw new Exception("JWT key not found")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GetDefaultRoleIdAsync()
        {
            var roleRepo = _unitOfWork.GetRepository<Role>();
            var defaultRole = await roleRepo.Entities.FirstOrDefaultAsync(r => r.RoleName == "User");
            return defaultRole?.Id.ToString() ?? throw new Exception("Default role not found.");
        }
    }
}
