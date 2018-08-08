using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Models;
using SharedCode.Helpers;
using Flurl;
using Client.Helpers;

namespace Client.ViewModels
{
    public class ClientAddCustomerViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;

       
        private bool cansave;
        public bool CanSave
        {
            get { return cansave; }
            set { SetProperty(ref cansave, value); }
        }
     

        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }


        public DelegateCommand RegisterCommand { get; set; }
        INavigationService _navigationService;

        public ClientAddCustomerViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            Customer = ClientModule.Customer;
            _navigationService = navigationService;
            RegisterCommand = new DelegateCommand(Register);

        }

        private async void Register()
        {

            try
            {
                if (!string.IsNullOrEmpty(Customer.Firstname) && !string.IsNullOrEmpty(Customer.Lastname) && !string.IsNullOrEmpty(Customer.Email) && !string.IsNullOrEmpty(Customer.Phone))
                {
                    Customer.RegistrationId = Settings.RegistrationId;
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Registering");
                    var register = await ServerPath.Path
                        .AppendPathSegment("/api/customers/postcustomer/FixmyPlace Support")
                        .WithOAuthBearerToken(ClientModule.AccessToken)
                        .PostJsonAsync(Customer);
                    if (register.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Customer = new Customer
                        {
                            Email = Customer.Email,
                            Lastname = Customer.Lastname,
                            Firstname = Customer.Firstname,
                            Phone = Customer.Phone,
                            CustomerId = Customer.CustomerId
                        };

                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await _navigationService.NavigateAsync("ClientDashboard");
                    }
                }
                else
                {
                   await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Required field(s) cannot be null.");
                }
            }
            catch (FlurlHttpException ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
            }

        }
    }
}
