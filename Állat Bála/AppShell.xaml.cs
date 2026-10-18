namespace PetManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(Routes.Edit, typeof(Views.EditPetPage));
        Routing.RegisterRoute(Routes.Detail, typeof(Views.PetDetailPage));
    }
}