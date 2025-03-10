namespace StudentMind.Services.Interfaces
{
    public interface IFirebaseAuthService
    {
        Task<string> SignInWithFirebaseAsync(string idToken);
    }
}
