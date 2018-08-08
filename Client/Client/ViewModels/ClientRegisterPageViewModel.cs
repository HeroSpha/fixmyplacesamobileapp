using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Helpers;
using Client.Helpers;
using Flurl.Http;
using SharedCode.Models;
using Flurl;

namespace Client.ViewModels
{
    public class ClientRegisterPageViewModel : BindableBase
    {
        private string idNumber;
        public string IdNumber
        {
            get { return idNumber; }
            set { SetProperty(ref idNumber, value); }
        }
        private string unit;
        public string Unit
        {
            get { return unit; }
            set { SetProperty(ref unit, value); }
        }
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
        private string _firstname;
        public string FirstName
        {
            get { return _firstname; }
            set
            {
                SetProperty(ref _firstname, value);
                Canregister = !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Phone);
            }
        }
        private string _lastname;
        public string LastName
        {
            get { return _lastname; }
            set
            {
                SetProperty(ref _lastname, value);
                Canregister = !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Phone);
            }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                SetProperty(ref _email, value);
                Canregister = !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Phone);
            }
        }
        private string _phone;
        public string Phone
        {
            get { return _phone; }
            set
            {
                SetProperty(ref _phone, value);
                Canregister = !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Phone);
            }
        }
        private bool _canregister;
        public bool Canregister
        {
            get { return _canregister; }
            set
            {
                SetProperty(ref _canregister, value);

            }
        }
        public DelegateCommand RegisterCommand { get; set; }
        INavigationService _navigationService;
       
        public ClientRegisterPageViewModel(INavigationService navigationService)
        {
           
            
            _navigationService = navigationService;
            RegisterCommand = new DelegateCommand(RegisterCustomer).ObservesCanExecute(() => Canregister);

        }

        private async void RegisterCustomer()
        {

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Registering");
                var register = await ServerPath.Path
                    .AppendPathSegment("/api/customers/postcustomer/" + ClientModule.Tenant).PostJsonAsync(new
                {
                   Email,
                    LastName,
                    FirstName,
                   Phone,
                   Unit,
                   IdNumber,
                    CustomerId = ClientModule.UserId
                });
                if (register.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Customer = new Customer
                    {
                        Email = Email,
                        Lastname = LastName,
                        Firstname = FirstName,
                        Phone = Phone,
                        Unit = Unit,
                        IdNumber = IdNumber,
                        CustomerId = Settings.UserId
                    };
                    
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await _navigationService.NavigateAsync("ClientDashboard");
                }
            }
            catch (FlurlHttpException ex)
            {
              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
            }

        }

       
    }
}
