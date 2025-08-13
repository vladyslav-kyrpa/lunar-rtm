namespace ServerApp.Services.Interfaces;

public interface IAuthService
{
    Task RegisterUser(string username, string password, string displayName, string email);
    Task<bool> LoginUser(string username, string password);
    Task LogoutUser();
}