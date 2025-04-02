namespace StudentMind.Services.Interfaces
{
    public interface IFirebaseAuthService
    {
        Task<string> SignInWithEmailPasswordAsync(string email, string password);
        Task RegisterUserAsync(string email, string password);
        Task SendPasswordResetEmailAsync(string email);
        Task ResetPasswordAsync(string oobCode, string newPassword);
    }
}
