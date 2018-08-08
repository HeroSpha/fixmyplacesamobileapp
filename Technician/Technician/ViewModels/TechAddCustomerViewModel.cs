using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using SharedCode.Models;
using Prism.Navigation;
using SharedCode.Helpers;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechAddCustomerViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;

        private string tenantName;
        public string TenantName
        {
            get { return tenantName; }
            set { SetProperty(ref tenantName, value); }
        }

        private string firstname;
        public string Firstname
        {
            get { return firstname; }
            set { SetProperty(ref firstname, value);
                CanSave = !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Email);
            }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value);
                CanSave = !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Email);
            }
        }
        private string phone;
        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value);
                CanSave = !string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Email);
            }
        }
        private bool cansave;
        public bool CanSave
        {
            get { return cansave; }
            set { SetProperty(ref cansave, value); }
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

        public TechAddCustomerViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            Provider = TechnicianModule.Provider;
            _navigationService = navigationService;
            RegisterCommand = new DelegateCommand(Register);

        }

        private async void Register()
        {

            try
            {
                if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Email))
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Registering");
                    var register = await ServerPath.Path
                        .AppendPathSegment("/api/customers/postcustomer/FixmyPlace Support")
                        .WithOAuthBearerToken(TechnicianModule.AccessToken).PostJsonAsync(new
                    {
                        Email = Email,
                        LastName = "Technician",
                        FirstName = Firstname,
                        Phone = Phone,
                        CustomerId = TechnicianModule.UserId
                    });
                    if (register.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Customer = new Customer
                        {
                            Email = Email,
                            Lastname = "Tenant",
                            Firstname = Firstname,
                            Phone = Phone,
                            CustomerId = TechnicianModule.UserId
                        };

                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await _navigationService.NavigateAsync("TechDashboard");
                    }
                }
                else
                {
                  await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Required field(s) cannot be null.");
                }
            }
            catch (FlurlHttpException ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
            }

        }
    }
}
