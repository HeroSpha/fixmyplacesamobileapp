using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Prism.Navigation;
using SharedCode.Helpers;
using Client.Helpers;
using Flurl.Http;
using System.Threading.Tasks;
using Flurl;

namespace Client.ViewModels
{
    public class ClientSearchProviderPageViewModel : BindableBase
    {
        private ObservableCollection<Property> _placeHolders;
        private IFlurlClient FlurlClient { get; set; }
        private ObservableCollection<Property> providers;
        public ObservableCollection<Property> Providers
        {
            get { return providers; }
            set { SetProperty(ref providers, value); }
        }
        INavigationService _navigationService;
        public ClientSearchProviderPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);

            _navigationService = navigationService;
            GetProviders().ConfigureAwait(true);
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
        private Property selectedProvider;
        public Property SelectedProvider
        {
            get { return selectedProvider; }
            set
            {
                SetProperty(ref selectedProvider, value);
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
        private async Task GetProviders()
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading...");
            var providers = await ServerPath.Path
                .AppendPathSegment("/account/gettenants")
                .WithOAuthBearerToken(ClientModule.AccessToken).GetJsonListAsync();
            if (providers != null)
            {
                var list = providers.Select(provider => new Property
                {
                    TenantName = provider.tenantName,
                   
                    Address = provider.address,
                    Description = provider.description,
                  
                    LookupId = provider.lookupId
                }).ToList();
                Providers = new ObservableCollection<Property>(list);
                _placeHolders = new ObservableCollection<Property>(Providers);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
        }
    }
}
