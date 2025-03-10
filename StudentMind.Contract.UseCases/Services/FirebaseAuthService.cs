using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Tokens;
using StudentMind.Core.Entity;
using StudentMind.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StudentMind.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace StudentMind.Services.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public FirebaseAuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<string> SignInWithFirebaseAsync(string idToken)
        {
            try
            {
                // ✅ Verify Firebase Token
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                string firebaseUserId = decodedToken.Uid;
                string? email = decodedToken.Claims.ContainsKey("email") ? decodedToken.Claims["email"].ToString() : null;
                string? fullName = decodedToken.Claims.ContainsKey("name") ? decodedToken.Claims["name"].ToString() : "Unknown";
                string? username = email?.Split('@')[0] ?? firebaseUserId;

                if (string.IsNullOrEmpty(email))
                {
                    throw new Exception("Firebase token does not contain an email.");
                }

                // ✅ Check if the user exists
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = (await userRepo.Entities.FirstOrDefaultAsync(u => u.Email == email));

                if (user == null)
                {
                    // ✅ Create a new user
                    user = new User
                    {
                        Username = username,
                        Email = email,
                        FullName = fullName,
                        RoleId = await GetDefaultRoleIdAsync() // Assign default role
                    };

                    await userRepo.InsertAsync(user);
                    await _unitOfWork.SaveAsync();
                }

                // ✅ Generate JWT Token
                return await GenerateJwtToken(user);
            }
            catch (FirebaseAuthException)
            {
                throw new Exception("Invalid Firebase token.");
            }
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("JWT key not found")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email ?? ""),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User") // Assign user role
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
            var defaultRole = (await roleRepo.Entities.FirstOrDefaultAsync(r => r.RoleName == "User"));
            return defaultRole?.Id.ToString() ?? throw new Exception("Default role not found.");
        }
    }
}
