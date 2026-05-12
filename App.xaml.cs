using GAAPerform.Services;
using GAAPerform.Views;

namespace GAAPerform;

public partial class App : Application
{
    private readonly DatabaseService _db;

    public App(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}