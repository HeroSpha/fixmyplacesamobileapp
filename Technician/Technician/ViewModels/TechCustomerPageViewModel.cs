using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Models;

namespace TechTechnician.ViewModels
{
    public class TechCustomerPageViewModel : BindableBase, INavigatingAware
    {
        private Customer _customer;
        public Customer Customer
        {
            get { return _customer; }
            set { SetProperty(ref _customer, value); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand MessageCommand { get; set; }
        public DelegateCommand CallCommand { get; set; }
        public DelegateCommand SendEmailCommand { get; set; }
        INavigationService _navigationService;
        public TechCustomerPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            MessageCommand = new DelegateCommand(Message);
            CallCommand = new DelegateCommand(Call);
            SendEmailCommand = new DelegateCommand(SendEmail);
        }

        private async void SendEmail()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("customer", Customer);
            await _navigationService.NavigateAsync("ClientEmailPage", para);
        }
        private async void Call()
        {
            try
            {
                //var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Standard call charges may apply, would you like to continue?", $"Call {Customer.FirstName}", "Yes", "No");
                //if (result)
                //{
                //    var callManager = CrossMessaging.Current.PhoneDialer;
                //    if (callManager.CanMakePhoneCall)
                //    {
                //        callManager.MakePhoneCall(Customer.PhoneNumber, Customer.FirstName);
                //    }
                //}
            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

        private async void Message()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Standard SMS may apply, would you like to continue?", $"SMS {Customer.Firstname}", "Yes", "No");
                if (result)
                {
                    NavigationParameters para = new NavigationParameters();
                    para.Add("customer", Customer);
                    await _navigationService.NavigateAsync("ClientSmsPage", para);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("customer"))
                {
                    Customer = ((Customer)parameters["customer"]);
                }
            }
        }
    }
}
