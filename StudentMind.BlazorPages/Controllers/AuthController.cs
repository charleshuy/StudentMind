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




    }
}
