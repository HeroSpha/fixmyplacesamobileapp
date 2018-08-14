using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using System.Threading.Tasks;
using SharedCode.Helpers;
using SharedCode.Models;
using Client.Helpers;
using Flurl.Http;
using System.Collections.ObjectModel;
using Flurl;

namespace Client.ViewModels
{
    public class ClientAddProviderViewModel : BindableBase, INavigatingAware
    {

      
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
 
        private ObservableCollection<Property> providers;
        public ObservableCollection<Property> Providers
        {
            get { return providers; }
            set { SetProperty(ref providers, value); }
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

        public DelegateCommand RequestCommand { get; set; }
        public DelegateCommand QRCodeCommand { get; set; }
        INavigationService _navigationService;
       
        public ClientAddProviderViewModel(INavigationService navigationService)
        {
            
            _navigationService = navigationService;
            SearchCommand = new DelegateCommand(Search).ObservesCanExecute(() => Cansearch);
            RequestCommand = new DelegateCommand(SendRequest);
            QRCodeCommand = new DelegateCommand(QRCode);
        }

        private async void Search()
        {
            await SearchProvider(LookupKey);
        }

        private async void QRCode()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            if (result != null)
            {
                LookupKey = result.Text;
                await SearchProvider(LookupKey);
            }


        }

        private async void SendRequest()
        {
            try
            {
                if(Providers != null)
                {
                    var _name = Providers.FirstOrDefault(p => p.TenantName.ToLower() == Provider.TenantName.ToLower());
                    if (_name != null)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
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
        private async Task LinkServiceProvider()
        {
            try
            {
                if (Provider.Setting.ExternalTenantAccount)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding Provider");
                    var request = await ServerPath.Path
                        .AppendPathSegment("/api/providers/add")
                        .WithOAuthBearerToken(ClientModule.AccessToken)
                        .PostJsonAsync(new
                        {
                            CustomerId = Settings.UserId,
                            Provider.PropertyId,
                            IsConfirmed = true
                        });
                    if (request.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await _navigationService.NavigateAsync("ClientProvidersPage");
                    }
                    else
                    {
                       await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error");
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
        private async Task SearchProvider(string LookupKey)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Searching...");

                var provider = await ServerPath.Path
                    .AppendPathSegment("/api/properties/getproprty/" + LookupKey)
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonAsync<Property>();
                if (provider != null)
                {
                    Provider = provider;

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); ;
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); ;
                    await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No match found.");
                }


            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading(); ;
                await   Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No match found.");

            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if(parameters.ContainsKey("providers"))
            {
                if (parameters.GetNavigationMode() == NavigationMode.New)
                {
                    try
                    {
                        var prov = parameters["providers"] as ObservableCollection<Property>;
                        Providers = new ObservableCollection<Property>(prov);
                    }
                    catch (Exception)
                    {

                        
                    }
                }
            }
        }
    }
}
