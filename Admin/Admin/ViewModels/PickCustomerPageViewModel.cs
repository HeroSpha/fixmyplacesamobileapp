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

namespace Admin.ViewModels
{
	public class PickCustomerPageViewModel : BindableBase
	{
        public ObservableCollection<Customer> _tenants { get; set; }
        private ObservableCollection<Customer> customers;
        public ObservableCollection<Customer> Tenants
        {
            get { return customers; }
            set { SetProperty(ref customers, value); }
        }
        private string searchKey;
        public string SearchKey
        {
            get { return searchKey; }
            set { SetProperty(ref searchKey, value);
                if(!string.IsNullOrEmpty(SearchKey))
                {
                    var _filtered = _tenants.Where(p => p.Firstname.ToLower().Contains(SearchKey.ToLower()) || p.Lastname.ToLower().Contains(SearchKey.ToLower()) || p.Unit.ToLower().Contains(SearchKey.ToLower()));
                }
                else
                {
                    Tenants = _tenants;
                }
            }
        }
        private Customer tenant;
        public Customer Tenant
        {
            get { return tenant; }
            set { SetProperty(ref tenant, value);
                if(Tenant != null)
                {
                    NavigationParameters para = new NavigationParameters
                    {
                        {"tenant", Tenant }
                    };
                    _navigationService.NavigateAsync("AdminAddVisitorPage", para);
                    Tenant = null;
                }
            }
        }
        INavigationService _navigationService;
        public PickCustomerPageViewModel(INavigationService navigationService)
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
                    .AppendPathSegment("/api/customers/getallcustomers/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<Customer>>();
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                Tenants = new ObservableCollection<Customer>(tenants);
                _tenants = new ObservableCollection<Customer>(Tenants);
                Tenant = null;
            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No tenants found");
            }
        }
    }
}
