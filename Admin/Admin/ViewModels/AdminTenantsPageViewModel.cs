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
	public class AdminTenantsPageViewModel : BindableBase
	{

        public ObservableCollection<Customer> _tenants { get; set; }
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
                        {"customer", Tenant }
                    };
                    _navigationService.NavigateAsync("AdminPostPage", para);
                    Tenant = null;
                }
            }
        }
        private string searchKey;
        public string Searchkey
        {
            get { return searchKey; }
            set { SetProperty(ref searchKey, value);
                if(!string.IsNullOrEmpty(Searchkey))
                {
                    var _filtered = Tenants.Where(p => p.Firstname.ToLower().Contains(Searchkey.ToLower()) || p.Lastname.ToLower().Contains(Searchkey.ToLower()) || p.Unit.ToLower().Contains(Searchkey.ToLower()));
                    Tenants = new ObservableCollection<Customer>(_filtered);
                }
                else
                {
                    Tenants = _tenants;
                }
            }
        }

        private void Search(string searchkey)
        {
            var _temp = Tenants.Where(p => p.Firstname.ToLower().Contains(searchkey.ToLower()));

        }

        INavigationService _navigationService;
        public AdminTenantsPageViewModel(INavigationService navigationService)
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
                Tenants = new ObservableCollection<Customer>(Tenants);
                _tenants = new ObservableCollection<Customer>(Tenants);
                Tenant = null;
            }
            catch (Exception ex)
            {

                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No tenants found");
            }
        }
    }
}
