using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using System.Collections.ObjectModel;
using Prism.Navigation;
using System.Threading.Tasks;
using Flurl.Http;
using SharedCode.Helpers;
using Client.Helpers;
using Flurl;

namespace Client.ViewModels
{
    public class ClientNotifyPageViewModel : BindableBase, INavigatingAware
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
                    _navigationService.NavigateAsync("ClientNotificationDetail", para);
                    Notification = null;
                }
            }
        }
        INavigationService _navigationService;
        public ClientNotifyPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            GetNotifications().ConfigureAwait(true);

            //Get notificantions
            //App.hubProxy.On<Notification>("addNotification", text =>
            //{
            //    Notifications.Add(new Notification { Title = text.Title, Message = text.Message, PostedOn = text.PostedOn, Priority = text.Priority });
            //});
        }
        private async Task GetNotifications()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading notifications");
                var notifications = await ServerPath.Path
                    .AppendPathSegment("/api/notications/getnotifications/" + ClientModule.Tenant)
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonAsync<List<Notification>>();
                Notifications = new ObservableCollection<Notification>(notifications);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
        }

        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                TenantName = parameters["tenantName"].ToString();
               
            }
        }
    }
}
