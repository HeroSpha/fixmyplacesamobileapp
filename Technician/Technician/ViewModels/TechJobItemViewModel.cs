using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Prism.Navigation;
using SharedCode.Helpers;
using Flurl.Http;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;

using Plugin.ExternalMaps;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechJobItemViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private bool locationAvailable;
        public bool LocationAvailable
        {
            get { return locationAvailable; }
            set { SetProperty(ref locationAvailable, value); }
        }
        private Issue _issue;
        public Issue Issue
        {
            get { return _issue; }
            set { SetProperty(ref _issue, value); }
        }
        public DelegateCommand AddDateCommand { get; set; }
        public DelegateCommand CostCommand { get; set; }
        public DelegateCommand CompleteCommmand { get; set; }
        public DelegateCommand CustomerCommand { get; set; }
        public DelegateCommand ImagesCommand { get; set; }
        INavigationService _navigationService;
       
        private bool _isComplete;
       

        public bool IsComplete
        {
            get { return _isComplete; }
            set { SetProperty(ref _isComplete, value); }
        }
        public TechJobItemViewModel(INavigationService navigationService)
        {
            if(TechnicianModule.Latitude != null)
            {
                locationAvailable = true;
            }
            else
            {
                locationAvailable = false;
            }
            ImagesCommand = new DelegateCommand(Images);
            AddDateCommand = new DelegateCommand(AddDate);
            CostCommand = new DelegateCommand(Cost);
            CompleteCommmand = new DelegateCommand(Complete);
            CustomerCommand = new DelegateCommand(CustomerView);
            LocationCommand = new DelegateCommand(Location);
            _navigationService = navigationService;


            //update issue

            TechnicianModule.hubProxy.On<Issue>("updateIssue", _issue =>
            {
                Issue = _issue;
            });
        }

        private async void Images()
        {
            NavigationParameters para = new NavigationParameters
            {
                {"issue", Issue }
            };
            await _navigationService.NavigateAsync("ImagesPage", para);
        }

        public DelegateCommand LocationCommand { get; set; }
        private async void CustomerView()
        {
           
          
            NavigationParameters para = new NavigationParameters();
            para.Add("customer", Issue.Customer);
            await _navigationService.NavigateAsync("TechCustomerPage", para);
        }

        private async void Location()
        {
           await CrossExternalMaps.Current.NavigateTo("Issue location", double.Parse(Issue.Latitude.ToString()), double.Parse(Issue.Longitude.ToString()));
        }

        private async void Complete()
        {
            var jobperformed = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("", "Job Performed", "Mark as complete", "Cancel", "Describe job performed", Acr.UserDialogs.InputType.Default);
            if(jobperformed.Ok)
            {
                Issue.JobPerformed = jobperformed.Value;
                Issue.IsResolved = true;
                Issue.Status = "Resolved";

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating");
                var updateResult = await ServerPath.Path
                    .AppendPathSegment("/api/issues/updateissue/" + TechnicianModule.TenantName).WithOAuthBearerToken(TechnicianModule.AccessToken).PutJsonAsync(Issue);
                if (updateResult.StatusCode == System.Net.HttpStatusCode.OK)
                {

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                else
                {
                   await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error");
                }
            }
        }

        private async void Cost()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("jobitem", Issue);
            await _navigationService.NavigateAsync("TechCostPage", para);
        }
        private async Task GetIssue(string IssueId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var _issue = await ServerPath.Path
                       .AppendPathSegment("/api/issues/getissue/" + IssueId + "/" + TechnicianModule.TenantName)
                       .GetJsonAsync<Issue>();
                if (_issue != null)
                {
                    Issue = _issue;
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
        private async void AddDate()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("item", Issue);
            await _navigationService.NavigateAsync("TechJobItemDatesPage", para);
        }
        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("jobitem"))
                {
                    var _issue = ((Issue)parameters["jobitem"]);
                    await GetIssue(_issue.IssueId);
                    Title = Issue.Title;
                    IsComplete = !Issue.IsResolved;

                }
            }
        }
    }
}
