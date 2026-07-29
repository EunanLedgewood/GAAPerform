using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _vm;

    public HistoryPage(HistoryViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
        Resources.Add("ExpandCollapseTextConverter", new ExpandCollapseTextConverter());
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}