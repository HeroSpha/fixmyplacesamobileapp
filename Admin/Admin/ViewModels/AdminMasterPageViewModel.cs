using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Admin.ViewModels
{
	public class AdminMasterPageViewModel : BindableBase
	{
        private string logo;
        public string Logo
        {
            get { return logo; }
            set { SetProperty(ref logo, value); }
        }
        private StartItem menuItem;
        public StartItem MenuItem
        {
            get { return menuItem; }
            set { SetProperty(ref menuItem, value);
                try
                {
                    if (MenuItem.Path == "PropertiesPage")
                        _navigationService.NavigateAsync("NavigationPage/" + MenuItem.Path);
                    else
                    _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/" + MenuItem.Path, useModalNavigation: true);
                    MenuItem = null;
                }
                catch (Exception ex)
                {
                }
            }
        }
        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }
        private ObservableCollection<StartItem> menuItems;
        public ObservableCollection<StartItem> MenuItems
        {
            get { return menuItems; }
            set { SetProperty(ref menuItems, value); }
        }
        INavigationService _navigationService;
        public AdminMasterPageViewModel(INavigationService navigationService)
        {
            Logo = AdminModule.Logo;
            Username = AdminModule.TenantName;
            _navigationService = navigationService;
            MenuItems = new ObservableCollection<StartItem>
            {
                new StartItem
                {
                    Name = "Issues",
                    Description ="User logged Issues",
                    Path = "AdminIssuesPage",
                    Icon = "md-assignment-late"
                },
                 new StartItem
                {
                    Name = "Tenants",
                    Description ="Manage Tenants",
                    Path = "AdminTenantsPage",
                    Icon ="md-people"
                },
                 new StartItem
                {
                    Name = "Job cards",
                    Description ="Create and manage jobs",
                    Path = "AdminJobcard",
                    Icon ="md-build"
                },
                 new StartItem
                {
                    Name = "Settings",
                    Description ="Manage application",
                    Path = "AdminSettingsPage",
                    Icon ="md-settings"
                },
                new StartItem
                {
                    Name = "Visitors",
                    Description ="Manage building access",
                    Path = "AdminVisitorPage",
                    Icon ="md-face"
                },
                  new StartItem
                {
                    Name = "Calendar",
                    Description ="View dates for all scheduled jobs",
                    Path = "AdminCalendar",
                    Icon ="ion-ios-calendar"
                },

                     new StartItem
                {
                    Name = "Notifications",
                    Description ="Notify users",
                    Path = "AdminNotificationPage",
                    Icon ="md-alarm"
                },

                     new StartItem
                {
                    Name = "Properties",
                    Description ="All properties ",
                    Path = "PropertiesPage",
                    Icon ="md-home"
                }
            };
        }
	}
}
