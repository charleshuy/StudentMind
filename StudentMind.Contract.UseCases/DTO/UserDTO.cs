using System.ComponentModel.DataAnnotations;

namespace StudentMind.Services.DTO
{
    public class UserDTO
    {
    }

    public class UserRequestDTO
    {
        public string? Username { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public string? FullName { get; set; }
        public required string RoleId { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; } = true;
    }
}
