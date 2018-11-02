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
using TechTechnician.Helpers;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechDashboardViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;

        private Property providers;
        public Property Providers
        {
            get { return providers; }
            set { SetProperty(ref providers, value); }
        }

        private Technicians technician;
        public Technicians Technician
        {
            get { return technician; }
            set { SetProperty(ref technician, value); }
        }
        private string _tenantName;
        public string TenantName
        {
            get { return _tenantName; }
            set { SetProperty(ref _tenantName, value); }
        }

        private ObservableCollection<StartItem> _startItem;
        public ObservableCollection<StartItem> StartItems
        {
            get { return _startItem; }
            set { SetProperty(ref _startItem, value); }
        }

        private string _logo;
        public string Logo
        {
            get { return _logo; }
            set { SetProperty(ref _logo, value); }
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
                    NavigationParameters para = new NavigationParameters();
                    TechnicianModule.Provider = Providers;
                    _navigationService.NavigateAsync(SelectedItem.Path);
                    SelectedItem = null;
                }
            }
        }

        public async Task UpdateRegistrationId()
        {
            var RegistrationId = Settings.RegistrationId;
            if (!string.IsNullOrEmpty(RegistrationId))
            {
                //update user
                try
                {
                    var result = await ServerPath.Path
                        .AppendPathSegment("/api/technicians/updatetechnician/" + TechnicianModule.TenantName)
                        .WithOAuthBearerToken(TechnicianModule.AccessToken)
                        .PostJsonAsync(new
                    {
                        Technician.Name,
                        Phone = Technician.Phone,
                       Technician.Email,
                       Technician.Description,
                        Technician.TechnicianId,
                        RegistrationId
                    });
                   
                }
                catch (Exception ex)
                {


                }
            }
        }
        private async Task GetTechnician()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading($"Signing to {TechnicianModule.TenantName}");
                var technician = await ServerPath.Path
                    .AppendPathSegment("/api/technicians/gettechnician/" + TechnicianModule.TenantName + "/" + TechnicianModule.UserId)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync<Technicians>();
                if (technician != null)
                {
                   
                    Technician = technician;
                    TechnicianModule.Technician = Technician;
                    Username = $"Welcome, {Technician.Name}";
                    await UpdateRegistrationId();
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Username = "You are not registered";
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("You are not registered.");
                await _navigationService.NavigateAsync("TechRegisterPage");
            }
        }

      
        public void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                Providers = parameters["provider"] as Property;
            }
            else if (parameters.GetNavigationMode() == NavigationMode.Back)
            {


            }
        }

        public DelegateCommand<StartItem> ItemTappedCommand { get; set; }
        INavigationService _navigationService;
        public TechDashboardViewModel(INavigationService navigationService)
        {
            
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;

            TenantName = TechnicianModule.TenantName;
            GetTechnician().ConfigureAwait(true);

            StartItems = new ObservableCollection<StartItem>
            {
                new StartItem
                {
                    Name = "Job cards",
                    Description ="User logged Issues",
                    Path = "TechJobsPage",
                    Icon = "ion-wrench"
                },
                new StartItem
                {
                    Name = "Quotations",
                    Description ="Get quotations from Service Provider",
                    Path = "TechQuotation",
                    Icon = "md-account-balance-wallet"
                },
                 new StartItem
                {
                    Name = "Details",
                    Description ="Service Provider Information",
                    Path = "TechProviderDetails",
                    Icon ="md-assignment"
                },
             
                 new StartItem
                {
                    Name = "Settings",
                    Description ="Manage application",
                    Path = "TechSettingsPage",
                    Icon ="md-settings"
                },
                new StartItem
                {
                    Name = "Calendar",
                    Description ="View dates for all scheduled jobs",
                    Path = "TechCalendarPage",
                    Icon ="ion-ios-calendar"
                }
            };


        }
    }
}
