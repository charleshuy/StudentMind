using StudentMind.Services.DTO;
using System.Security.Claims;

namespace StudentMind.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByIdAsync(string userId);
        Task<PaginatedList<UserDTO>> GetUsersAsync(int pageNumber, int pageSize);
        Task<List<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> CreateUserAsync(UserRequestDTO userDto);
        Task<bool> UpdateUserAsync(string userId, UserRequestDTO userDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<UserDTO?> GetProfileAsync();
        string? GetUserIdFromToken();
    }
}
