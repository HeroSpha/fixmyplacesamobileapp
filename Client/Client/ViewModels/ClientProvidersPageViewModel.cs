using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Helpers;
using Client.Helpers;
using Xamarin.Forms;
using System.Threading.Tasks;
using Flurl;

namespace Client.ViewModels
{
    public class ClientProvidersPageViewModel : BindableBase
    {
        private  IFlurlClient FlurlClient { get; set; }
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
                    var item = _placeHolders.Where(p => p.TenantName.ToLower().Contains(SearchKey.ToLower()) || p.LookupId.Contains(SearchKey)).ToList();
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
                    ClientModule.Provider = SelectedProvider;
                    ClientModule.Tenant = SelectedProvider.TenantName;
                    NavigationParameters param = new NavigationParameters();
                    param.Add("provider", SelectedProvider);
                    _navigationService.NavigateAsync("ClientDashboard", param);
                    SelectedProvider = null;
                }

            }
        }
        public DelegateCommand AddProviderCommand { get; set; }
        public DelegateCommand SearchCommand { get; set; }

        INavigationService _navigationService;
       
        public ClientProvidersPageViewModel(INavigationService navigationService)
        {
            ClientModule.UserId = Settings.UserId;
            ClientModule.AccessToken = Settings.AccessToken;
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            AddProviderCommand = new DelegateCommand(AddProvicer);
            SearchCommand = new DelegateCommand(Search);

            GetProviders().ConfigureAwait(true);
        }

        private async void Search()
        {
            await _navigationService.NavigateAsync("ClientSearchProviderPage");
        }
        private async void AddProvicer()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("providers", Providers);
            await _navigationService.NavigateAsync("ClientAddProvider", para);
       
        }
        private async Task GetProviders()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading tenant providers...");
                var providers = await ServerPath.Path
                    .AppendPathSegment("/api/providers/getcustproviders/" + ClientModule.UserId)
                    .WithOAuthBearerToken(ClientModule.AccessToken).GetJsonListAsync();
                if (providers != null)
                {
                    var list = providers.Select(provider => new Property
                    {
                        TenantName = provider.tenantName,
                      
                        Address = provider.address,
                        Description = provider.description,
                         Parent = new Shared.Models.Parent {  Logo = provider.parent.logo},
                        LookupId = provider.lookupId
                    }).ToList();
                    Providers = new ObservableCollection<Property>(list);
                    _placeHolders = new ObservableCollection<Property>(Providers);
                   
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
       
        }

       
    }
}
