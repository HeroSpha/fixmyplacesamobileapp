using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Models;

namespace Client.ViewModels
{
    public class ClientNotificationDetailViewModel : BindableBase, INavigatingAware
    {
        private Notification notification;
        public Notification Notification
        {
            get { return notification; }
            set { SetProperty(ref notification, value); }
        }
      
        public ClientNotificationDetailViewModel()
        {

        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Notification = parameters["noti"] as Notification;
            }
        }
    }
}
