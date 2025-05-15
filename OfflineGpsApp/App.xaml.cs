using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace OfflineGpsApp
{
    public partial class App : Application
    {
        public App(System.IServiceProvider serviceProvider)
        {
            InitializeComponent();

            //MainPage = new AppShell();
            //MainPage = new NavigationPage(new MainPage());

            MainPage = new Microsoft.Maui.Controls.NavigationPage(serviceProvider.GetRequiredService<MainPage>());
        }
    }
}
