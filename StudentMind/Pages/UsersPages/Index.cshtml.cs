using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentMind.Core.Entity;
using StudentMind.Core.Interfaces;

namespace StudentMind.Pages.UsersPages
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public IndexModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IList<User> User { get;set; } = default!;

        public async Task OnGetAsync()
        {
            User = await _unitOfWork.GetRepository<User>().Entities
                .Include(u => u.Role).ToListAsync();
        }
    }
}
