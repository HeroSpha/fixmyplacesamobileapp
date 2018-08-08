using System;
using System.Threading.Tasks;
using Client.Helpers;
using Client.Views;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Prism.Ioc;
using Prism.Modularity;
using SharedCode.Helpers;
using SharedCode.Models;

namespace Client
{
    public class ClientModule : IModule
    {
        
        public static string UserId { get; set; }
        public static string AccessToken { get; set; }
        public static HubProxy hubProxy { get; set; }
        public static string Tenant { get; set; }
        public static Property Provider { get; internal set; }
        public static Customer Customer { get; set; }
        public static string Role { get; internal set; }

       
      
        private async Task MapSignalR(string UserId)
        {
            try
            {
                var hubConnection = new HubConnection(ServerPath.Path + "/signalr");
                hubProxy = hubConnection.CreateHubProxy("MyHub") as HubProxy;
                hubProxy.On<Issue>("updateIssue", text =>
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("Issue updated.");
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
            containerRegistry.RegisterForNavigation<ClientDashboard>();
            containerRegistry.RegisterForNavigation<ClientProvidersPage>();
            containerRegistry.RegisterForNavigation<ClientAddProvider>();
            containerRegistry.RegisterForNavigation<ClientRegisterPage>();
            containerRegistry.RegisterForNavigation<ClientPostPage>();
            containerRegistry.RegisterForNavigation<ClientIssuesPage>();
            containerRegistry.RegisterForNavigation<ClientNotificationDetail>();
            containerRegistry.RegisterForNavigation<ClientNotifyPage>();
            containerRegistry.RegisterForNavigation<ClientSearchProviderPage>();
            containerRegistry.RegisterForNavigation<ClientLocationPage>();
            containerRegistry.RegisterForNavigation<ClientDetails>();
            containerRegistry.RegisterForNavigation<ClientSettingsPage>();
            containerRegistry.RegisterForNavigation<ClientSupportPage>();
            containerRegistry.RegisterForNavigation<ClientEditPage>();
            containerRegistry.RegisterForNavigation<ClientAddCustomer>();
            containerRegistry.RegisterForNavigation<PostPage>();
            containerRegistry.RegisterForNavigation<ClientVisitorPage>();
            containerRegistry.RegisterForNavigation<ClientAddVisitor>();
            containerRegistry.RegisterForNavigation<ClientIssuePage>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            Role = Settings.Role;
            UserId = Settings.UserId;
            AccessToken = Settings.AccessToken;
        }
    }
}
