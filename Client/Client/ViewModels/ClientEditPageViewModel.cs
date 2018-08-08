using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Flurl.Http;
using SharedCode.Helpers;
using Flurl;

namespace Client.ViewModels
{
    public class ClientEditPageViewModel : BindableBase
    {
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
        private IFlurlClient FlurlClient;
      
        public DelegateCommand UpdateCommand { get; set; }
        public ClientEditPageViewModel()
        {
            Customer = ClientModule.Customer;
            FlurlClient = new FlurlClient(ServerPath.Path);
            UpdateCommand = new DelegateCommand(UpdateAsync);
        }

        private async void UpdateAsync()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/customers/updatecustomer/" + ClientModule.Provider.TenantName).WithOAuthBearerToken(ClientModule.AccessToken).PostJsonAsync(new
                {
                    ClientModule.Customer.CustomerId,
                    Customer.Email,
                    Customer.Firstname,
                    Customer.Lastname,
                    Customer.Phone,
                    Customer.RegistrationId,
                    Customer.Unit,
                    Customer.IdNumber
                    
                });
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
            }

        }
       
    }
}
