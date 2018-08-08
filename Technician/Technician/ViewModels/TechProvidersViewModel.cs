using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Helpers;
using SharedCode.Models;
using Flurl.Http;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using TechTechnician.Helpers;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechProvidersViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;
        private string _title = "Favorites";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private ObservableCollection<Property> _placeHolders;
        private ObservableCollection<Property> _providers;
        public ObservableCollection<Property> Providers
        {
            get { return _providers; }
            set { SetProperty(ref _providers, value); }
        }
        private string _searchKey;
        public string SearchKey
        {
            get { return _searchKey; }
            set
            {
                SetProperty(ref _searchKey, value);
                if (SearchKey != null)
                {
                    var item = _placeHolders.Where(p => p.TenantName.ToLower().Contains(SearchKey.ToLower())).ToList();
                    Providers = new ObservableCollection<Property>(item);
                }
                else
                    Providers = _placeHolders;
            }
        }
        private Property _selectedProvider;
        public Property SelectedProvider
        {
            get { return _selectedProvider; }
            set
            {
                SetProperty(ref _selectedProvider, value);
                if (SelectedProvider != null)
                {
                    TechnicianModule.TenantName = SelectedProvider.TenantName;

                    NavigationParameters param = new NavigationParameters();
                    param.Add("provider", SelectedProvider);
                    _navigationService.NavigateAsync("TechDashboard", param);
                    SelectedProvider = null;
                }

            }
        }
        public DelegateCommand AddProviderCommand { get; set; }

        INavigationService _navigationService;
        public TechProvidersViewModel(INavigationService navigationService)
        {
            TechnicianModule.Role = Settings.Role;
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            AddProviderCommand = new DelegateCommand(AddProvicer);
            TechnicianModule.UserId = Settings.UserId;
            GetProviders();
        }


        private async void GetProviders()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading Technician providers");
                var providers = await ServerPath.Path
                    .AppendPathSegment("/api/providers/gettechproviders/" + TechnicianModule.UserId ).WithOAuthBearerToken(TechnicianModule.AccessToken).GetJsonListAsync();
                if (providers.Count > 0)
                {
                    var list = providers.Select(provider => new Property
                    {
                        TenantName = provider.tenantName,
                        LookupId = provider.lookupId,
                        Address = provider.address,
                        Description = provider.description,
                        Parent = new Shared.Models.Parent { Logo = provider.parent.logo }
                       
                    }).ToList();
                    Providers = new ObservableCollection<Property>(list);
                    _placeHolders = new ObservableCollection<Property>(Providers);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private async void AddProvicer()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("providers", Providers);
            await _navigationService.NavigateAsync("TechAddProvider", para);
        }
    }

   
}
