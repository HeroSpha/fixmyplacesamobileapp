using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Prism.Ioc;
using Prism.Modularity;
using SharedCode.Helpers;
using SharedCode.Models;
using Technician.Views;
using TechTechnician.Helpers;
using TechTechnician.Views;



namespace TechTechnician
{
    public class TechnicianModule : IModule
    {
        public static SharedCode.Models.Property Provider { get; set; }
        public static  Technicians Technician { get; set; }
        public static HubProxy hubProxy { get; set; }
        public static string AccessToken { get; set; }
        public static string UserId { get; set; }
        public static string Role { get; set; }
        public static string TenantName { get; set; }

        //location 
        public static double? Latitude { get; set; }
        public static double? Longitude { get; set; }
     
        private async Task MapSignalR(string UserId)
        {
            try
            {
                var hubConnection = new HubConnection(ServerPath.Path + "/signalr");
                hubProxy = hubConnection.CreateHubProxy("MyHub") as HubProxy;
                hubProxy.On<JobCard>("addJobCard", text =>
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("New jobcard created.");
                });
                await hubConnection.Start();
                await hubProxy.Invoke("subscribe", UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TechProviders>();
            containerRegistry.RegisterForNavigation<TechDashboard>();
            containerRegistry.RegisterForNavigation<TechAddProvider>();
            containerRegistry.RegisterForNavigation<TechCalendarPage>();
            containerRegistry.RegisterForNavigation<TechCostPage>();
            containerRegistry.RegisterForNavigation<TechCustomerPage>();
            containerRegistry.RegisterForNavigation<TechJobItem>();
            containerRegistry.RegisterForNavigation<TechJobItemDatesPage>();
            containerRegistry.RegisterForNavigation<TechJobItemsPage>();
            containerRegistry.RegisterForNavigation<TechJobsPage>();
            containerRegistry.RegisterForNavigation<TechPasswordPage>();
            containerRegistry.RegisterForNavigation<TechProfilePage>();
            containerRegistry.RegisterForNavigation<TechProviderDetails>();
            containerRegistry.RegisterForNavigation<TechRegisterPage>();
            containerRegistry.RegisterForNavigation<TechAddDate>();
            containerRegistry.RegisterForNavigation<TechSupportPage>();
            containerRegistry.RegisterForNavigation<TechAddCostPage>();

            containerRegistry.RegisterForNavigation<TechSettingsPage>();
            containerRegistry.RegisterForNavigation<TechEditPage>();
            containerRegistry.RegisterForNavigation<TechAddCustomer>();
            containerRegistry.RegisterForNavigation<TechQuotation>();
            containerRegistry.RegisterForNavigation<TechAddQuotation>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            Role = Settings.Role;
            AccessToken = Settings.AccessToken;
            UserId = Settings.UserId;
            MapSignalR(UserId).ConfigureAwait(false);
        }
    }
}
