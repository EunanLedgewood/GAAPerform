using GAAPerform.Models;
using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class SessionDetailPage : ContentPage
{
    private readonly SessionDetailViewModel _vm;
    private readonly TrainingDay _day;

    public SessionDetailPage(SessionDetailViewModel vm, TrainingDay day)
    {
        InitializeComponent();
        _vm = vm;
        _day = day;
        BindingContext = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync(_day);
    }
}