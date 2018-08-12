using Prism.Unity;
using Fixmyplacemobileapp.Views;
using Xamarin.Forms;
using Prism.Modularity;
using System;
using FormsPlugin.Iconize;
using Flurl.Http;
using Fixmyplacemobileapp.Helpers;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Fixmyplacemobileapp.Models;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter;
using Prism.Ioc;
using Prism;
using Com.OneSignal;
using Com.OneSignal.Abstractions;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Fixmyplacemobileapp
{
    public partial class App : PrismApplication
    {
        public static string MobileServiceUrl = ServerPath.Path;
        public static string UserId { get; internal set; }

        public static string RegistrationId { get; set; }
        public static HubProxy hubProxy { get; set; }
        public static string Access_Token { get; internal set; }
        public static FlurlClient FlurlClient { get; internal set; }
        public static Property Tenant { get; set; }
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
           
        }

       

        protected override void OnInitialized()
        {
            InitializeComponent();
            try
            {
                AppCenter.Start("android=705828a1-9926-4cce-badc-5a6dfaa3dc49;" + "uwp={Your UWP App secret here};" +
                       "ios=ba09390c-6962-47c8-a55b-b44acabf38ab;",
                       typeof(Analytics), typeof(Crashes));
                OneSignal.Current.StartInit("8c740c9e-3185-4e06-bcb6-709f13a9e1c6")
               .EndInit();
                OneSignal.Current.IdsAvailable((idsAvailable, another) =>
                {
                    Settings.RegistrationId = idsAvailable;
                });
                NavigationService.NavigateAsync("MainPage");
            }
            catch (Exception ex)
            {
                NavigationService.NavigateAsync("MainPage");
                // throw;
            }
        }
         

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            Type adminType = typeof(Admin.AdminModule);
            moduleCatalog.AddModule(
              new ModuleInfo(adminType, adminType.Name, InitializationMode.OnDemand));
            Type clientType = typeof(Client.ClientModule);
            moduleCatalog.AddModule(
              new ModuleInfo(clientType, clientType.Name, InitializationMode.OnDemand));

            Type techType = typeof(TechTechnician.TechnicianModule);
            moduleCatalog.AddModule(
              new ModuleInfo(techType, techType.Name, InitializationMode.OnDemand));
            Type visitorType = typeof(Visitor.VisitorModule);
            moduleCatalog.AddModule(
              new ModuleInfo(visitorType, visitorType.Name, InitializationMode.OnDemand));

        }


        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPage>();
            containerRegistry.RegisterForNavigation<SignUpPage>();
            containerRegistry.RegisterForNavigation<IconNavigationPage>(nameof(NavigationPage));
            containerRegistry.RegisterForNavigation<IconTabbedPage>(nameof(TabbedPage));
            containerRegistry.RegisterForNavigation<SignUpPage>();
            containerRegistry.RegisterForNavigation<CarPage>();
            containerRegistry.RegisterForNavigation<ResetPage>();
            containerRegistry.RegisterForNavigation<ChangePasswordPage>();
            containerRegistry.RegisterForNavigation<CarPage>();
            containerRegistry.RegisterForNavigation<ImagesPage>();
           
        }
    }
}
