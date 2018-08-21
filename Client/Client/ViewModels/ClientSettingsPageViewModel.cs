using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Client.Helpers;
using System.Collections.ObjectModel;
using SharedCode.Models;

namespace Client.ViewModels
{
    public class ClientSettingsPageViewModel : BindableBase
    {
        private ObservableCollection<StartItem> items;
        public ObservableCollection<StartItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }
        public string Role { get; set; }
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }


        private StartItem selectedItem;
        public StartItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                SetProperty(ref selectedItem, value);
                try
                {
                    if (SelectedItem != null)
                    {
                        if (SelectedItem.Path == "SignOut")
                        {
                            SignOut();
                            SelectedItem = null;
                        }
                        else
                        {
                            
                            _navigationService.NavigateAsync(SelectedItem.Path);
                            SelectedItem = null;
                        }
                    }
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
        }

        private async void SignOut()
        {
            var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Sign out from FixmyPlace Dashboard?", "", "Sign Out", "Cancel");
            if (result)
            {
                
                await _navigationService.NavigateAsync("myapp:///MainPage");
            }

        }

        INavigationService _navigationService;
        public ClientSettingsPageViewModel(INavigationService navigationService)
        {

            _navigationService = navigationService;
            Customer = ClientModule.Customer;

            Role = Settings.Role;
            Items = new ObservableCollection<StartItem>
            {
                new StartItem{Icon = "fa-sign-out", Name = "Sign Out", Path="SignOut"},
                 new StartItem{Icon = "ion-edit", Name = "Edit information", Path="ClientEditPage"},
                  new StartItem{Icon = "ion-locked", Name = "Change Password", Path="ChangePasswordPage"}
            };
        }
    }
}
