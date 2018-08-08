using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Models;
using SharedCode.Helpers;
using Flurl.Http;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using TechTechnician.Helpers;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechAddProviderViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private string _lookupKey;
        public string LookupKey
        {
            get { return _lookupKey; }
            set
            {
                SetProperty(ref _lookupKey, value);
                Cansearch = !string.IsNullOrEmpty(LookupKey);
            }
        }


        private bool _canSearch;
        public bool Cansearch
        {
            get { return _canSearch; }
            set { SetProperty(ref _canSearch, value); }
        }
        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set { SetProperty(ref _visible, value); }
        }
        private Property _providers;
        public Property Provider
        {
            get { return _providers; }
            set
            {
                SetProperty(ref _providers, value);
                Visible = Provider != null;
            }
        }

        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand QRCodeCommand { get; set; }
        public DelegateCommand RequestCommand { get; set; }
        INavigationService _navigationService;
        public TechAddProviderViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            SearchCommand = new DelegateCommand(Search).ObservesCanExecute(() => Cansearch);
            QRCodeCommand = new DelegateCommand(QRCode);
            RequestCommand = new DelegateCommand(SendRequest);
        }
        private ObservableCollection<Property> providers;
        public ObservableCollection<Property> Providers
        {
            get { return providers; }
            set { SetProperty(ref providers, value); }
        }
        private async void Search()
        {
            await SearchProvider(LookupKey);
        }

        private async void QRCode()
        {
            try
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null)
                {
                    LookupKey = result.Text;
                    await SearchProvider(LookupKey);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private async Task LinkServiceProvider()
        {
            try
            {
                if (Provider.Setting.ExternalTechAccount)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding Provider");
                    var request = await ServerPath.Path
                        .AppendPathSegment("/api/providers/add")
                        .WithOAuthBearerToken(TechnicianModule.AccessToken)
                        .PostJsonAsync(new
                        {
                            CustomerId = Settings.UserId,
                            Provider.PropertyId,
                            IsConfirmed = false
                        });
                    if (request.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await _navigationService.NavigateAsync("TechProviders");
                    }
                    else
                    {
                      await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error");
                    }
                }
                else
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("External account link not supported, please contact service provider for more information.");
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        private async void SendRequest()
        {
            try
            {
                if (Providers != null)
                {
                    var _name = Providers.FirstOrDefault(p => p.TenantName.ToLower() == Provider.TenantName.ToLower());
                    if (_name != null)
                    {
                      await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Service provider already added.");
                    }
                    else
                    {
                        await LinkServiceProvider();
                      
                    }
                }
                else
                {
                    await LinkServiceProvider();
                }

            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
            }
        }

        private async Task SearchProvider(string LookupKey)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Searching...");

                var provider = await ServerPath.Path
                    .AppendPathSegment("/api/properties/getproprty/" + LookupKey)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync();
                if (provider != null)
                {
                    Provider = new Property
                    {
                        PropertyId = provider.propertyId,
                        TenantName = provider.tenantName,
                        Address = provider.address,
                        Description = provider.description,
                        Parent = new Shared.Models.Parent {  Logo = provider.parent.logo},
                        Setting = new Shared.Models.Setting { ExternalTechAccount = provider.setting.externalTechAccount}

                    };

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                else
                {
                   await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No match found.");
                }


            }
            catch (Exception ex)
            {
              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No match found.");

            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
           if(parameters.ContainsKey("providers"))
            {
                if(parameters.GetNavigationMode() == NavigationMode.New)
                {
                    try
                    {
                        var _items = parameters["providers"] as ObservableCollection<Property>;
                        Providers = new ObservableCollection<Property>(_items);
                    }
                    catch (Exception)
                    {

                       
                    }
                }
            }
        }
    }
}
