using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class OnboardingPage : ContentPage
{
    public OnboardingPage(OnboardingViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}