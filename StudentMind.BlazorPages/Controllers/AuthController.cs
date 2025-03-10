using Microsoft.AspNetCore.Mvc;
using StudentMind.Services.Interfaces;

namespace StudentMind.BlazorPages.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public AuthController(IFirebaseAuthService firebaseAuthService)
        {
            _firebaseAuthService = firebaseAuthService;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
                return BadRequest(new { error = "Invalid token" });

            try
            {
                var jwtToken = await _firebaseAuthService.SignInWithFirebaseAsync(request.IdToken);
                return Ok(new { token = jwtToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // DTO for Request
        public class GoogleLoginRequest
        {
            public string IdToken { get; set; }
        }

    }
}
