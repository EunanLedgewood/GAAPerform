using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(AuthViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}