namespace FuelLog;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(Routes.Edit, typeof(Views.EditRefuelPage));
        Routing.RegisterRoute(Routes.Stats, typeof(Views.StatsPage));
    }
}