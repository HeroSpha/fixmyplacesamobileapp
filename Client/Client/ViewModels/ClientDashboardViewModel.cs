using Prism.Mvvm;
using System;
using SharedCode.Models;
using System.Collections.ObjectModel;
using Prism.Navigation;
using System.Threading.Tasks;
using Flurl.Http;
using SharedCode.Helpers;
using Client.Helpers;
using Flurl;
using Prism.Commands;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace Client.ViewModels
{
    public class ClientDashboardViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
        private Property providers;
        public Property Provider
        {
            get { return providers; }
            set { SetProperty(ref providers, value); }
        }
       

        private ObservableCollection<StartItem> _startItem;
        public ObservableCollection<StartItem> StartItems
        {
            get { return _startItem; }
            set { SetProperty(ref _startItem, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }
       
        private StartItem _selectedItem;
        public StartItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                if (SelectedItem != null)
                {
                    if (Customer == null && SelectedItem.Name == "Profile")
                    {
                        NavigationParameters para = new NavigationParameters();
                        para.Add("IsRegistered", false);
                        _navigationService.NavigateAsync(SelectedItem.Path, para);
                        SelectedItem = null;
                    }
                    else
                    {

                        NavigationParameters para = new NavigationParameters();
                        para.Add("tenantName", Provider.TenantName);
                        para.Add("customer", Customer);
                        _navigationService.NavigateAsync(SelectedItem.Path, para);
                        SelectedItem = null;
                    }
                }
            }
        }
        private async Task UpdateRegistration()
        {
            try
            {
                var installationId = Settings.RegistrationId;
                if(!string.IsNullOrEmpty(installationId))
                {
                    var result = await ServerPath.Path
                        .AppendPathSegment("/api/customers/updatecustomer/" + ClientModule.Provider.TenantName).WithOAuthBearerToken(ClientModule.AccessToken).PostJsonAsync(new
                    {
                        ClientModule.Customer.CustomerId,
                        Customer.Email,
                        Customer.Firstname,
                        Customer.Lastname,
                        Customer.Phone,
                        RegistrationId = installationId,
                        Customer.Unit,
                        Customer.IdNumber

                    });
                }
              
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
            }
        }
        private async Task GetCustomer()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading($"Signing to {Provider.TenantName}");
                var customer = await ServerPath.Path
                    .AppendPathSegment("/api/customers/getcustomer/" + ClientModule.UserId + "/" + ClientModule.Tenant)
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonAsync<Customer>();
                if (customer != null)
                {
                   
                    Customer = customer;
                    
                    ClientModule.Customer = Customer;

                    Username = $"Welcome, {Customer.Firstname}";
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                    await UpdateRegistration();
                    await MapSignalR(Customer.CustomerId);
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Singing failed");
                }

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("You are not registered.");
                Username = "You are not registered";
                NavigationParameters para = new NavigationParameters();
                para.Add("tenantName", Provider.TenantName);
                await _navigationService.NavigateAsync("ClientRegisterPage");
            }
        }
        INavigationService _navigationService;
       
        public ClientDashboardViewModel(INavigationService navigationService)
        {
            
            _navigationService = navigationService;
            Provider = ClientModule.Provider;
            Title = Provider.TenantName;
            ClientModule.UserId = Settings.UserId;
            ClientModule.Role = Settings.Role;
            FlurlClient = new FlurlClient(ServerPath.Path);

            GetCustomer().ConfigureAwait(true);



            StartItems = new ObservableCollection<StartItem>
            {
                 new StartItem
                {
                    Name = "Log Issue",
                    Description ="Log issues / complaints",
                    Path = "ClientPostPage",
                    Icon = "md-add-box"
                },
                new StartItem
                {
                    Name = "Issues",
                    Description ="Tenant logged Issues and complaints",
                    Path = "ClientIssuesPage",
                    Icon = "md-warning"
                },

                  new StartItem
                {
                    Name = "Visitors",
                    Description ="Manage visitors",
                    Path = "ClientVisitorPage",
                    Icon ="md-face"
                },
                  new StartItem
                {
                    Name = "Notifications",
                    Description ="Receive notifications from service provider",
                    Path = "ClientNotifyPage",
                    Icon ="md-alarm"
                },
                    new StartItem
                {
                    Name = "Settings",
                    Description ="Manage your profile",
                    Path = "ClientSettingsPage",
                    Icon ="md-settings"
                },
                    new StartItem
                {
                    Name = "Details",
                    Description ="Property details",
                    Path = "ClientDetails",
                    Icon ="md-description"
                },
                    new StartItem
                {
                    Name = "Properties",
                    Description ="All linked properties",
                    Path = "ClientProvidersPage",
                    Icon ="md-home"
                }
            };


        }
        private async Task MapSignalR(string UserId)
        {
            try
            {
                var hubConnection = new HubConnection(ServerPath.Path + "/signalr");
                ClientModule.hubProxy = hubConnection.CreateHubProxy("MyHub") as HubProxy;
                ClientModule.hubProxy.On<JobCard>("addIssue", text =>
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("New issue added.");
                });
                await hubConnection.Start();
                await ClientModule.hubProxy.Invoke("subscribe", UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }


    }
}
