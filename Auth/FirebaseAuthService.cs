using Firebase.Auth;
using Firebase.Auth.Providers;

namespace GAAPerform.Auth;

public class FirebaseAuthService
{
    private readonly FirebaseAuthClient _client;
    private static AppSettings? _settings;
    private DateTime? _lastPasswordResetSent;

    private static AppSettings Settings => _settings ??= AppSettings.Load();

    public FirebaseAuthService()
    {
        var config = new FirebaseAuthConfig
        {
            ApiKey = Settings.Firebase.ApiKey,
            AuthDomain = Settings.Firebase.AuthDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        };
        _client = new FirebaseAuthClient(config);
    }

    public bool IsLoggedIn => _client.User is not null;
    public string? CurrentUserId => _client.User?.Uid;
    public string? CurrentUserEmail => _client.User?.Info?.Email;

    public async Task<string> GetTokenAsync()
    {
        var token = await _client.User.GetIdTokenAsync();
        return token;
    }

    public async Task<(bool success, string? error)> RegisterAsync(string email, string password)
    {
        try
        {
            await _client.CreateUserWithEmailAndPasswordAsync(email, password);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool success, string? error)> LoginAsync(string email, string password)
    {
        try
        {
            await _client.SignInWithEmailAndPasswordAsync(email, password);
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public Task SignOutAsync()
    {
        _client.SignOut();
        return Task.CompletedTask;
    }

    public async Task<(bool success, string? error)> SendPasswordResetAsync(string email)
    {
        // Check 5 minute cooldown
        if (_lastPasswordResetSent.HasValue)
        {
            var elapsed = DateTime.Now - _lastPasswordResetSent.Value;
            if (elapsed.TotalMinutes < 5)
            {
                var remaining = (int)(5 - elapsed.TotalMinutes) + 1;
                return (false, $"Please wait {remaining} minutes before requesting another reset email.");
            }
        }

        try
        {
            await _client.ResetEmailPasswordAsync(email);
            _lastPasswordResetSent = DateTime.Now;
            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
}