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
        _vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(WeekViewModel.SelectedDay) && _vm.SelectedDay is not null)
        {
            var detailVm = IPlatformApplication.Current!.Services
                .GetRequiredService<SessionDetailViewModel>();
            var detailPage = new SessionDetailPage(detailVm, _vm.SelectedDay);
            await Navigation.PushAsync(detailPage);
            _vm.SelectedDay = null;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}