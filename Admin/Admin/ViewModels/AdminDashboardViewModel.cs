using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Prism.Navigation;
using System.Threading.Tasks;
using Flurl.Http;
using SharedCode.Helpers;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Admin.Helpers;
using Flurl;
using Shared.Models;

namespace Admin.ViewModels
{
    public class AdminDashboardViewModel : BindableBase, INavigatingAware
    {

        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private Manager manager;
        public Manager Manager
        {
            get { return manager; }
            set { SetProperty(ref manager, value); }
        }
        private Property property;
        public Property Property
        {
            get { return property; }
            set { SetProperty(ref property, value); }
        }
        private IFlurlClient FlurlClient;
        private ObservableCollection<StartItem> startItems;
        public ObservableCollection<StartItem> StartItems
        {
            get { return startItems; }
            set { SetProperty(ref startItems, value); }
        }
        public DelegateCommand SignOutCommand { get; set; }
        private StartItem _selectedItem;
        public StartItem SelectedItem
        {
           
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
                if(Property != null)
                {
                    if (SelectedItem != null && Property.TenantName != null)
                    {
                        NavigationParameters para = new NavigationParameters();

                        _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/" + SelectedItem.Path, useModalNavigation: true);
                        SelectedItem = null;
                    }
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Your are not logged in, please log or register to login.");
                }
                
            }
        }
        INavigationService _navigationService;
        public AdminDashboardViewModel(INavigationService navigationService)
        {
            
            Username = AdminModule.TenantName;
            SignOutCommand = new DelegateCommand(SignOut);
            Property = AdminModule.Provider;
            
            AdminModule.Role = Settings.Role;
            AdminModule.UserId = Settings.UserId;
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            GetManager().ContinueWith(async reg =>
            {
                await UpdateRegistrationId();
            });
            StartItems = new ObservableCollection<StartItem>
            {
                new StartItem
                {
                    Name = "Issues",
                    Description ="User logged Issues",
                    Path = "AdminIssuesPage",
                    Icon = "md-assignment-late"
                },
                 new StartItem
                {
                    Name = "Tenants",
                    Description ="Manage Tenants",
                    Path = "AdminTenantsPage",
                    Icon ="md-people"
                },
                 new StartItem
                {
                    Name = "Job cards",
                    Description ="Create and manage jobs",
                    Path = "AdminJobcard",
                    Icon ="md-build"
                },
                 
                new StartItem
                {
                    Name = "Visitors",
                    Description ="Manage building access",
                    Path = "AdminVisitorPage",
                    Icon ="md-face"
                },
                  new StartItem
                {
                    Name = "Calendar",
                    Description ="View dates for all scheduled jobs",
                    Path = "AdminCalendar",
                    Icon ="ion-ios-calendar"
                },
                 
                     new StartItem
                {
                    Name = "Notifications",
                    Description ="Notify users",
                    Path = "AdminNotificationPage",
                    Icon ="md-alarm"
                },
                     new StartItem
                {
                    Name = "Settings",
                    Description ="Manage application",
                    Path = "AdminSettingsPage",
                    Icon ="md-settings"
                }
            };
        }
        private async Task GetManager()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var manager = await ServerPath.Path
                    .AppendPathSegment("/api/manager/getpropertymanager/" + AdminModule.UserId)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<Manager>();
                if (manager != null)
                {
                    Manager = manager;
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                } 
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                throw new ApplicationException(ex.Message);
            }
        }
        private async void SignOut()
        {
            var signOut = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Sign out?", "", "Sign out", "Cancel", null);
            if(signOut)
            {
                await _navigationService.NavigateAsync("MainPage");
            }
        }

        public async Task UpdateRegistrationId()
        {
            try
            {
                var RegistrationId = Settings.RegistrationId;
                if (!string.IsNullOrEmpty(RegistrationId) && Manager.RegistrationId != RegistrationId)
                {
                    //update user
                    try
                    {
                        Manager.RegistrationId = RegistrationId;
                        var updated = await ServerPath.Path
                            .AppendPathSegment("/api/manager/edit")
                            .WithOAuthBearerToken(AdminModule.AccessToken)
                            .PostJsonAsync(Manager);
                        if (updated.StatusCode == System.Net.HttpStatusCode.OK)
                        {

                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {

                throw new ApplicationException("Update failed");
            }
        }
        public async Task SetProperty()
        {
            try
            {
                AdminModule.TenantName = Property.TenantName;
                Title = Property.TenantName;
                Username = $"Welcome, {Property.TenantName}";
                await MapSignalR(Property.TenantName);
            }
            catch (Exception ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.InnerException.Message); 
            }
           
        }
        private async Task MapSignalR(string UserId)
        {
            try
            {
                var hubConnection = new HubConnection(ServerPath.Path + "/signalr");
                AdminModule.hubProxy = hubConnection.CreateHubProxy("MyHub") as HubProxy;
                AdminModule.hubProxy.On<JobCard>("addIssue", text =>
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("New issue added.");
                });
                await hubConnection.Start();
                await AdminModule.hubProxy.Invoke("subscribe", UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
           if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Property = parameters["property"] as Property;
                await SetProperty();
            }
        }
    }
}
