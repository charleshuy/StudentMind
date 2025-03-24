namespace StudentMind.Services.Interfaces
{
    public interface IFirebaseAuthService
    {
        Task<string> SignInWithEmailPasswordAsync(string email, string password);
    }
}
