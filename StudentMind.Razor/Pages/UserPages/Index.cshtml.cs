using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Services.DTO;
using StudentMind.Services.Interfaces;

namespace StudentMind.Razor.Pages.UserPages
{
    [Authorize(AuthenticationSchemes = "Jwt", Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public IReadOnlyCollection<UserDTO> Users { get; set; } = new List<UserDTO>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 5)
        {
            var paginatedUsers = await _userService.GetUsersAsync(pageNumber, pageSize);
            Users = paginatedUsers.Items;
            CurrentPage = paginatedUsers.PageNumber;
            TotalPages = paginatedUsers.TotalPages;
        }
    }
}
