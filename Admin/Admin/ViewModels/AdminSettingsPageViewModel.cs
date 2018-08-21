using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Prism.Navigation;
using Admin.Helpers;

namespace Admin.ViewModels
{
    public class AdminSettingsPageViewModel : BindableBase
    {
        private ObservableCollection<StartItem> items;
        public ObservableCollection<StartItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }
        public string Role { get; set; }
        private Property provider;
        public Property Provider
        {
            get { return provider; }
            set { SetProperty(ref provider, value); }
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
                            NavigationParameters para = new NavigationParameters();
                            para.Add("provider", Provider);
                           // _navigationService.GoBackToRootAsync();
                            _navigationService.NavigateAsync(SelectedItem.Path, para);
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
        public AdminSettingsPageViewModel(INavigationService navigationService)
        {

            _navigationService = navigationService;
            Provider = AdminModule.Provider;
            
            Role = AdminModule.Role;
            Items = new ObservableCollection<StartItem>
            {
                new StartItem{Icon = "fa-sign-out", Name = "Sign Out", Path="SignOut"},
                 new StartItem{Icon = "ion-edit", Name = "Edit information", Path="AdminEditPage"},
                  new StartItem{Icon = "ion-locked", Name = "Change Password", Path="ChangePasswordPage"}
            };
        }
    }
}
