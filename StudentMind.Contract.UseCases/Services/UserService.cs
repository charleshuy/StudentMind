using Microsoft.Extensions.Configuration;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;


namespace StudentMind.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        

        public UserService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<bool> UpdateUser(int userId, UserRequestDTO userDto)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var existingUser = await userRepo.GetByIdAsync(userId);
            if (existingUser == null)
                return false;

            existingUser.FullName = userDto.FullName;
            existingUser.Email = userDto.Email;
            existingUser.Status = userDto.Status;
            existingUser.Gender = userDto.Gender;

            await userRepo.UpdateAsync(existingUser);
            await _unitOfWork.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteUser(int userId)
        {
            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(userId);
            if (user == null)
                return false;

            await userRepo.DeleteAsync(user);
            await _unitOfWork.SaveAsync();

            return true;
        }
    }
}
