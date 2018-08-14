using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Prism.Navigation;
using SharedCode.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminNotificationPageViewModel : BindableBase
    {
        private string tenantName;
        public string TenantName
        {
            get { return tenantName; }
            set { SetProperty(ref tenantName, value); }
        }
        private IFlurlClient FlurlClient;
        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get { return _notifications; }
            set { SetProperty(ref _notifications, value); }
        }

        private Notification notification;
        public Notification Notification
        {
            get { return notification; }
            set
            {
                SetProperty(ref notification, value);
                if (Notification != null)
                {
                    NavigationParameters para = new NavigationParameters();
                    para.Add("noti", Notification);
                    _navigationService.NavigateAsync("AdminNotoficationDetail", para);
                    Notification = null;
                }
            }
        }
        INavigationService _navigationService;
        public DelegateCommand AddNotificationCommand { get; set; }
        public AdminNotificationPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            GetNotifications().ConfigureAwait(true);
            AddNotificationCommand = new DelegateCommand(Add);
            AddNotification();
            //Get notificantions
            //App.hubProxy.On<Notification>("addNotification", text =>
            //{
            //    Notifications.Add(new Notification { Title = text.Title, Message = text.Message, PostedOn = text.PostedOn, Priority = text.Priority });
            //});
        }
        private void AddNotification()
        {
            AdminModule.hubProxy.On<Notification>("AdminaddNotification", notification =>
            {
                Notifications.Add(notification);
            });
        }
        private async void Add()
        {
            await _navigationService.NavigateAsync("AdminAddNotification");
        }

        private async Task GetNotifications()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading notifications");
                var notifications = await ServerPath.Path
                    .AppendPathSegment("/api/notications/getnotifications/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<Notification>>();
                Notifications = new ObservableCollection<Notification>(notifications);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
        }

      
    }
}
