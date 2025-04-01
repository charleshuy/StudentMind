using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentMind.Core.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentMind.Razor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public string? UserName { get; private set; }
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            var token = HttpContext?.Request.Cookies["JWT_Token"];

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                UserName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                UserName = UserName.Split("@")[0];
            }
        }
    }
}
