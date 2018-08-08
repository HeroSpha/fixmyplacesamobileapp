using Prism.Ioc;
using Prism.Modularity;
using Visitor.Views;
using Visitor.ViewModels;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Visitor.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using Flurl;
using Flurl.Http;

namespace Visitor
{
    public class VisitorModule : IModule
    {
        public static SharedCode.Models.Property Provider { get; set; }
        public static HubProxy hubProxy { get; set; }
        public static string AccessToken { get; set; }
        public static string UserId { get; set; }
        public static string Role { get; set; }
        public static string TenantName { get; set; }
        public async void OnInitialized(IContainerProvider containerProvider)
        {
            Role = Settings.Role;
            AccessToken = Settings.AccessToken;
            UserId = Settings.UserId;
            await MapSignalR(UserId);
        }
       
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<VisitorsPage>();
            containerRegistry.RegisterForNavigation<AddVisitorPage>();
            containerRegistry.RegisterForNavigation<VisitorTenantPage>();
            containerRegistry.RegisterForNavigation<SecVisitorDetail>();
        }
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
    }
}
