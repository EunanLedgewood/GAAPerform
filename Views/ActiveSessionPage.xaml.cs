using GAAPerform.Models;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class ActiveSessionPage : ContentPage
{
    private readonly ActiveSessionViewModel _vm;

    public ActiveSessionPage(ActiveSessionViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;

        Resources.Add("BoolToCheckBgConverter", new BoolToCheckBgConverter());
    }

    public void LoadSession(TrainingDay day, SessionDetail detail)
    {
        _vm.LoadSession(day, detail);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _vm.Cleanup();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_vm.IsFinished)
        {
            await Navigation.PopAsync();
        }
    }
}