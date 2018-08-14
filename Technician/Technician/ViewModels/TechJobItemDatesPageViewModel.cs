using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechJobItemDatesPageViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private DateStamp dateStamp;
        public DateStamp DateStamp
        {
            get { return dateStamp; }
            set
            {
                SetProperty(ref dateStamp, value);
                IsVisible = DateStamp == null;
                CanManipulate = DateStamp != null;
            }
        }
        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }
        private bool canManipulate;
        public bool CanManipulate
        {
            get { return canManipulate; }
            set { SetProperty(ref canManipulate, value); }
        }
        private string title;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        INavigationService _navigationService;
        public DelegateCommand AddDateCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand UpdateCommand { get; set; }
      
        public TechJobItemDatesPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            AddDateCommand = new DelegateCommand(AddDate);
            DeleteCommand = new DelegateCommand(Delete);
            UpdateCommand = new DelegateCommand(Update);
            _navigationService = navigationService;
            UpdateDate();
          
        }
        private void UpdateDate()
        {
            TechnicianModule.hubProxy.On<DateStamp>("addDate", date =>
            {
                DateStamp = date;
            });
        }
        private void UpdateDate(DateStamp obj)
        {
            DateStamp = obj;
        }

        private async void Update()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("update", DateStamp);
            await _navigationService.NavigateAsync("TechAddDate", para);
        }

        private async void Delete()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete date stamp?", "Delete", "Delete", "Cancel");
                if (result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Deleting...");
                    var delete = await ServerPath.Path
                        .AppendPathSegment("/api/calendar/delete/" + TechnicianModule.TenantName + "/" + DateStamp.DateStampId).DeleteAsync();
                    if (delete.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        DateStamp = null;
                        await _navigationService.GoBackAsync();
                    }
                }
            }
            catch (Exception)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                throw;
            }
        }

        private Issue issue;
        public Issue Issue
        {
            get { return issue; }
            set { SetProperty(ref issue, value); }
        }
        private async void AddDate()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("jobitem", Issue);
            await _navigationService.NavigateAsync("TechAddDate", para);
        }

        private async Task GetDate()
        {
            try
            {
                DateStamp _stamp = new DateStamp();
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Get date");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/calendar/jobitem/" + TechnicianModule.TenantName + "/" + Issue.IssueId)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync<DateStamp>();
                if (result != null)
                {
                    _stamp.DateStampId = result.DateStampId;
                    _stamp.Description = result.Description;
                    _stamp.EndDate = result.EndDate;
                    _stamp.StartDate = result.StartDate;
                    _stamp.Title = result.Title;

                    DateStamp = _stamp;
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                }




            }
            catch (FlurlHttpException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                IsVisible = true;
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No date found.");
                //return null;
            }
        }
        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                Issue = ((Issue)parameters["item"]);
                if (issue != null)
                {
                    await GetDate();
                    Title = Issue.Title;
                }

            }
        }
    }
}
