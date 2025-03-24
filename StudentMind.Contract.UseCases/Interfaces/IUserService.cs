using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> UpdateUser(int userId, UserRequestDTO userDto);
        Task<bool> DeleteUser(int userId);
    }
}
