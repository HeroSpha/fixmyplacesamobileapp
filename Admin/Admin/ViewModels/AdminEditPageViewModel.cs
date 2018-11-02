using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Helpers;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Models;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminEditPageViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private Property provider;
        public Property Provider
        {
            get { return provider; }
            set { SetProperty(ref provider, value); }
        }
        public DelegateCommand UpdateCommand { get; set; }
        public AdminEditPageViewModel()
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            UpdateCommand = new DelegateCommand(UpdateAsync);
        }

        private async void UpdateAsync()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/tenant/update").WithOAuthBearerToken(AdminModule.AccessToken).PostJsonAsync(new
                {
                    Id = AdminModule.Provider.PropertyId,
                  
                    Provider.Address,
                    
                    description = Provider.Description,
                    isPublic = Provider.IsPublic
                });
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
            }

        }


        public void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                Provider = parameters["provider"] as Property;

            }
        }
    }
}
