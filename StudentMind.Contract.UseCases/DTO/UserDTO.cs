using System.ComponentModel.DataAnnotations;

namespace StudentMind.Services.DTO
{
    public class UserDTO
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Gender { get; set; } = "Male";
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTimeOffset CreatedTime { get; set; }
        public DateTimeOffset LastUpdatedTime { get; set; }
    }

    public class UserRequestDTO
    {
        public string? Username { get; set; }
        [Required]
        public required string Email { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public required string RoleId { get; set; }
        public string? Gender { get; set; }
        public bool Status { get; set; } = true;
    }
}
