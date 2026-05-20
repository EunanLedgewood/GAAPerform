using Android.Telephony;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GAAPerform.Auth;
using IntelliJ.Lang.Annotations;

namespace GAAPerform.ViewModels;

public partial class AuthViewModel : ObservableObject
{
    private readonly FirebaseAuthService _auth;
    private readonly FirestoreService _firestore;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private bool isPlayer = true;
    [ObservableProperty] private bool isCoach = false;
    [ObservableProperty] private bool isLoginMode = true;
    [ObservableProperty] private bool isRegisterMode = false;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool hasError = false;
    [ObservableProperty] private bool isBusy = false;
    [ObservableProperty] private string toggleModeText = "Don't have an account? Register";

    public AuthViewModel(FirebaseAuthService auth, FirestoreService firestore)
    {
        _auth = auth;
        _firestore = firestore;
    }

    [RelayCommand]
    private void SelectRole(string role)
    {
        IsPlayer = role == "Player";
        IsCoach = role == "Coach";
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsLoginMode = !IsLoginMode;
        IsRegisterMode = !IsRegisterMode;
        ToggleModeText = IsLoginMode
            ? "Don't have an account? Register"
            : "Already have an account? Login";
        ErrorMessage = string.Empty;
        HasError = false;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please enter your email and password.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        var (success, error) = await _auth.LoginAsync(Email, Password);

        if (success)
        {
            var token = await _auth.GetTokenAsync();
            var role = await _firestore.GetUserRoleAsync(_auth.CurrentUserId!, token) ?? "Player";
            Preferences.Set("user_role", role);
            Preferences.Set("is_logged_in", true);
            Application.Current!.Windows[0].Page = new AppShell();
        }
        else
        {
            ErrorMessage = "Invalid email or password. Please try again.";
            HasError = true;
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Please fill in all fields.";
            HasError = true;
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            HasError = true;
            return;
        }

        if (Password.Length < 6)
        {
            ErrorMessage = "Password must be at least 6 characters.";
            HasError = true;
            return;
        }

        IsBusy = true;
        HasError = false;

        var (success, error) = await _auth.RegisterAsync(Email, Password);

        if (success)
        {
            var token = await _auth.GetTokenAsync();
            var role = IsCoach ? "Coach" : "Player";
            await _firestore.SaveUserProfileAsync(_auth.CurrentUserId!, Email, role, token);
            Preferences.Set("user_role", role);
            Preferences.Set("is_logged_in", true);
            Application.Current!.Windows[0].Page = new AppShell();
        }
        else
        {
            ErrorMessage = error ?? "Registration failed. Please try again.";
            HasError = true;
        }

        IsBusy = false;
    }
}