using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class ReadinessPage : ContentPage
{
    private readonly ReadinessViewModel _vm;

    public ReadinessPage(ReadinessViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}
