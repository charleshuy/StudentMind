using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDTO?> GetUserByIdAsync(string userId)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null) return null;

            return MapToUserDTO(user);
        }

        public async Task<PaginatedList<UserDTO>> GetUsersAsync(int pageNumber, int pageSize)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var query = userRepo.Entities.Include(u => u.Role).AsNoTracking()
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Username = u.Username,
                    Gender = u.Gender,
                    RoleId = u.RoleId,
                    RoleName = u.Role!.RoleName ?? "",
                    Status = u.Status,
                    CreatedTime = u.CreatedTime,
                    LastUpdatedTime = u.LastUpdatedTime
                });

            return await PaginatedList<UserDTO>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var query = userRepo.Entities.AsNoTracking()
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email ?? "",
                    Username = u.Username,
                    Gender = u.Gender,
                    RoleId = u.RoleId,
                    Status = u.Status,
                    CreatedTime = u.CreatedTime,
                    LastUpdatedTime = u.LastUpdatedTime
                });

            return await query.ToListAsync();
        }

        public async Task<UserDTO> CreateUserAsync(UserRequestDTO userDto)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = new User
            {
                Username = userDto.Username ?? "",
                Email = userDto.Email,
                Password = userDto.Password, // You may want to hash it later
                FullName = userDto.FullName ?? "",
                RoleId = userDto.RoleId,
                Gender = userDto.Gender ?? "Male",
                Status = userDto.Status
            };

            await userRepo.InsertAsync(user);
            await _unitOfWork.SaveAsync();

            return MapToUserDTO(user);
        }

        public async Task<bool> UpdateUserAsync(string userId, UserRequestDTO userDto)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            user.Username = userDto.Username ?? user.Username;
            user.Email = userDto.Email;
            user.Password = userDto.Password; // Hash if needed
            user.FullName = userDto.FullName ?? user.FullName;
            user.RoleId = userDto.RoleId;
            user.Gender = userDto.Gender ?? user.Gender;
            user.Status = userDto.Status;
            user.LastUpdatedTime = DateTimeOffset.UtcNow;

            await userRepo.UpdateAsync(user);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null) return false;

            await userRepo.DeleteAsync(user);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                Username = user.Username,
                Gender = user.Gender,
                RoleId = user.RoleId,
                Status = user.Status,
                CreatedTime = user.CreatedTime,
                LastUpdatedTime = user.LastUpdatedTime
            };
        }
        public async Task<UserDTO?> GetProfileAsync()
        {
            var jwtToken = _httpContextAccessor.HttpContext?.Request.Cookies["JWT_Token"];

            if (string.IsNullOrEmpty(jwtToken))
            {
                return null; // User not logged in
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return null; // Invalid token
            }

            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(userId);

            return user == null ? null : MapToUserDTO(user);
        }

        public string? GetUserIdFromToken()
        {
            // Get the JWT token from the cookies
            var jwtToken = _httpContextAccessor.HttpContext?.Request.Cookies["JWT_Token"];

            if (string.IsNullOrEmpty(jwtToken))
            {
                return null; // User not logged in
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);

            // Extract the UserId claim from the token
            var userId = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            return userId;
        }
    }
}
