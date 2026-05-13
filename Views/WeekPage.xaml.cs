using GAAPerform.Models;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class WeekPage : ContentPage
{
    private readonly WeekViewModel _vm;

    public WeekPage(WeekViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        Resources.Add("SessionIconConverter", new SessionIconConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}