using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Visitor.ViewModels
{
	public class VisitorTenantPageViewModel : BindableBase
	{
        private ObservableCollection<Customer> customers;
        public ObservableCollection<Customer> Tenants
        {
            get { return customers; }
            set { SetProperty(ref customers, value); }
        }
        private Customer tenant;
        public Customer Tenant
        {
            get { return tenant; }
            set
            {
                SetProperty(ref tenant, value);
                if (Tenant != null)
                {
                    NavigationParameters para = new NavigationParameters
                    {
                        {"tenant", Tenant }
                    };
                    _navigationService.NavigateAsync("AddVisitorPage", para);
                    Tenant = null;
                }
            }
        }
        INavigationService _navigationService;
        public VisitorTenantPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GetTenants();
        }
        public async void GetTenants()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var tenants = await ServerPath.Path
                    .AppendPathSegment("/api/customers/getallcustomers/" + VisitorModule.TenantName)
                    .WithOAuthBearerToken(VisitorModule.AccessToken)
                    .GetJsonAsync<List<Customer>>();
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                Tenants = new ObservableCollection<Customer>(tenants);
                Tenant = null;
            }
            catch (Exception ex)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No tenants found");
            }
        

    }
	}
}
