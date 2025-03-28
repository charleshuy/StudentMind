﻿using StudentMind.Services.DTO;

namespace StudentMind.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO?> GetUserByIdAsync(string userId);
        Task<PaginatedList<UserDTO>> GetUsersAsync(int pageNumber, int pageSize);
        Task<UserDTO> CreateUserAsync(UserRequestDTO userDto);
        Task<bool> UpdateUserAsync(string userId, UserRequestDTO userDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}
