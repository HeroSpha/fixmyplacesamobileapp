using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using Flurl.Http;
using SharedCode.Helpers;
using Prism.Navigation;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminAddNotificationViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;
        private ObservableCollection<string> _prioties;
        public ObservableCollection<string> Priorities
        {
            get { return _prioties; }
            set { SetProperty(ref _prioties, value);
                
            }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set
            {
                SetProperty(ref title, value);
                CanPost = !string.IsNullOrEmpty(Title) & !string.IsNullOrEmpty(SelectedPriority) && !string.IsNullOrEmpty(Message);
            }
        }
        private string message;
        public string Message
        {
            get { return message; }
            set
            {
                SetProperty(ref message, value);
                CanPost = !string.IsNullOrEmpty(Title) & !string.IsNullOrEmpty(SelectedPriority) && !string.IsNullOrEmpty(Message);
            }
        }
        private string selectedPriority;
        public string SelectedPriority
        {
            get { return selectedPriority; }
            set
            {
                SetProperty(ref selectedPriority, value);
                CanPost = !string.IsNullOrEmpty(Title) & !string.IsNullOrEmpty(SelectedPriority) && !string.IsNullOrEmpty(Message);
            }
        }
        public bool CanPost { get; set; }
        public DelegateCommand PostCommand { get; set; }

        INavigationService _navigationService;
        public AdminAddNotificationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            FlurlClient = new FlurlClient(ServerPath.Path);
            PostCommand = new DelegateCommand(Post).ObservesCanExecute(() => CanPost);
            Priorities = new ObservableCollection<string>
            {
                "High",
                "Medium",
                "Low"
            };
        }

        private async void Post()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Posting");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/notications/send/" + AdminModule.TenantName)
                    .PostJsonAsync(new 
                {
                    message = Message,
                    title = Title,
                    priority = SelectedPriority,
                    postedOn = DateTime.Now
                });

                if(result.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Notification published.");
                    Message = string.Empty;
                    Title = string.Empty;
                    SelectedPriority = null;
                    await _navigationService.NavigateAsync("AdminNotificationPage");

                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }
    }
}
