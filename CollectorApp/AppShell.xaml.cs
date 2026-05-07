using CollectorApp.Models;
using CollectorApp.Views;

namespace CollectorApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("result", typeof(ScanResultPage));
        //Routing.RegisterRoute("document", typeof(DocumentPage));
    }
}
