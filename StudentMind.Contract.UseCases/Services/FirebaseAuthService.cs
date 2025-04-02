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

                // Check email verification status
                var isEmailVerified = await IsEmailVerifiedAsync(idToken);

                if (!isEmailVerified)
                    throw new Exception("Email is not verified. Please check your inbox and verify your email.");

                // Check if user exists in the database
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.Entities.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    user = new User
                    {
                        Username = email.Split('@')[0],
                        Email = email,
                        Password = password,
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

        private async Task<bool> IsEmailVerifiedAsync(string idToken)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            var accountInfoUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

            var payload = new { idToken };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(accountInfoUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to retrieve account information.");

            var responseData = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
            var users = responseData.GetProperty("users");

            if (users.GetArrayLength() > 0)
            {
                return users[0].GetProperty("emailVerified").GetBoolean();
            }

            return false;
        }


        public async Task RegisterUserAsync(string email, string password)
        {
            try
            {
                var apiKey = _configuration["Firebase:ApiKey"];
                var signUpUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";

                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(signUpUrl, content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failed to register user. Please try again.");

                var responseData = JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync());
                var idToken = responseData.GetProperty("idToken").GetString();

                // Send verification email
                await SendEmailVerificationAsync(idToken);

                // Optional: Additional logic to handle user data in your database

                // Check if user exists
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.Entities.FirstOrDefaultAsync(u => u.Email == email);

                if (user != null)
                {
                    // Create a new user
                    user = new User
                    {
                        Username = email.Split('@')[0],
                        Email = email,
                        Password = password,
                        FullName = "Unknown",
                        RoleId = await GetDefaultRoleIdAsync()
                    };

                    await userRepo.InsertAsync(user);
                    await _unitOfWork.SaveAsync();
                }

            }
            catch (Exception ex)
            {
                throw new Exception($"Registration failed: {ex.Message}");
            }
        }

        public async Task SendEmailVerificationAsync(string idToken)
        {
            try
            {
                var apiKey = _configuration["Firebase:ApiKey"];
                var verificationUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

                var payload = new
                {
                    requestType = "VERIFY_EMAIL",
                    idToken = idToken
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(verificationUrl, content);

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Failed to send verification email.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Email verification failed: {ex.Message}");
            }
        }

        public async Task SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var apiKey = _configuration["Firebase:ApiKey"];
                var resetPasswordUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={apiKey}";

                var payload = new
                {
                    requestType = "PASSWORD_RESET",
                    email = email
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(resetPasswordUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to send password reset email.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Password reset email sending failed: {ex.Message}");
            }
        }

        public async Task ResetPasswordAsync(string oobCode, string newPassword)
        {
            try
            {
                var apiKey = _configuration["Firebase:ApiKey"];
                var resetPasswordUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:resetPassword?key={apiKey}";

                var payload = new
                {
                    oobCode = oobCode,  // Verification code from the email
                    newPassword = newPassword
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(resetPasswordUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to reset password.");
                }

                // Optionally: Handle any post-reset actions, such as notifying the user
            }
            catch (Exception ex)
            {
                throw new Exception($"Password reset failed: {ex.Message}");
            }
        }

        public async Task<string> AdminLoginAsync(string email, string password)
        {
            try
            {
                // Check if user exists in the database
                var userRepo = _unitOfWork.GetRepository<User>();
                var user = await userRepo.Entities.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    throw new Exception("User not found.");

                // Check if the user is an admin
                if (user.Role?.RoleName != "Admin")
                    throw new Exception("You do not have admin access.");

                // Verify password (you should implement hashing and comparison of stored password)
                if (user.Password != password) // Replace this with proper password comparison (e.g., Hashing)
                    throw new Exception("Invalid password.");

                // Generate JWT Token
                return await GenerateJwtToken(user);
            }
            catch (Exception ex)
            {
                throw new Exception($"Admin login failed: {ex.Message}");
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
