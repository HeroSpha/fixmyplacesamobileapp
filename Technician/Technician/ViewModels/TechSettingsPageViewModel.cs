using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using System.Collections.ObjectModel;
using Prism.Navigation;
using TechTechnician.Helpers;

namespace TechTechnician.ViewModels
{
    public class TechSettingsPageViewModel : BindableBase
    {
        private ObservableCollection<StartItem> items;
        public ObservableCollection<StartItem> Items
        {
            get { return items; }
            set { SetProperty(ref items, value); }
        }
        public string Role { get; set; }
        private Technicians technician;
        public Technicians Technician
        {
            get { return technician; }
            set { SetProperty(ref technician, value); }
        }


        private StartItem selectedItem;
        public StartItem SelectedItem
        {
            get { return selectedItem; }
            set { SetProperty(ref selectedItem, value);
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
                            para.Add("tech", Technician);
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
            if(result)
            {
                await _navigationService.NavigateAsync("MainPage");
            }
          
        }

        INavigationService _navigationService;
        public TechSettingsPageViewModel(INavigationService navigationService)
        {
            
            _navigationService = navigationService;
            Technician = TechnicianModule.Technician;
            Role = TechnicianModule.Role;
            Items = new ObservableCollection<StartItem>
            {
                new StartItem{Icon = "fa-sign-out", Name = "Sign Out", Path="SignOut"},
                new StartItem{Icon = "ion-edit", Name = "Edit information", Path="TechEditPage"},
                new StartItem{Icon = "ion-locked", Name = "Change Password", Path="ChangePasswordPage"}
            };
        }
     
    }
}
