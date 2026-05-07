using GAAPerform.ViewModels;

namespace GAAPerform.Views;

public partial class LogPage : ContentPage
{
    private readonly LogViewModel _vm;

    public LogPage(LogViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = vm;
    }
}