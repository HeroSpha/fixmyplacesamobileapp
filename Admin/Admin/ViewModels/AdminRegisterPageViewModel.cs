using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Helpers;
using SharedCode.Models;
using Admin.Helpers;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminRegisterPageViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;

        private string tenantName;
        public string TenantName
        {
            get { return tenantName; }
            set { SetProperty(ref tenantName, value); }
        }

        private Property provider;
        public Property Provider
        {
            get { return provider; }
            set { SetProperty(ref provider, value); }
        }

        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
       
     
        public DelegateCommand RegisterCommand { get; set; }
        INavigationService _navigationService;

        public AdminRegisterPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            Provider = AdminModule.Provider;
            _navigationService = navigationService;
            RegisterCommand = new DelegateCommand(RegisterCustomer);

        }

        private async void RegisterCustomer()
        {

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Registering");
                var register = await ServerPath.Path
                    .AppendPathSegment("/api/customers/postcustomer/FixmyPlace Support").PostJsonAsync(new
                {
                 
                    LastName = "Tenant",
                    FirstName = Provider.TenantName,
                  
                    CustomerId = Provider.PropertyId
                });
                if (register.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Customer = new Customer
                    {
                        
                        Lastname = "Tenant",
                        Firstname = Provider.TenantName,
                       
                        CustomerId = Provider.PropertyId.ToString()
                    };

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await _navigationService.NavigateAsync("Dashboard");
                }
            }
            catch (FlurlHttpException ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
            }

        }

       
    }
}
