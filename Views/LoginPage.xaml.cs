using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(AuthViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        var vm = BindingContext as GAAPerform.ViewModels.AuthViewModel;
        if (string.IsNullOrWhiteSpace(vm?.Email))
        {
            await DisplayAlertAsync("Reset Password", "Please enter your email address first.", "OK");
            return;
        }

        var auth = IPlatformApplication.Current!.Services
            .GetRequiredService<GAAPerform.Auth.FirebaseAuthService>();

        var (success, error) = await auth.SendPasswordResetAsync(vm.Email);

        if (success)
            await DisplayAlertAsync("Reset Password", $"Password reset email sent to {vm.Email}. Check your inbox.", "OK");
        else
            await DisplayAlertAsync("Error", error ?? "Failed to send reset email.", "OK");
    }
}