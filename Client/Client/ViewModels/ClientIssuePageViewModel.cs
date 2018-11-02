using Flurl;
using Flurl.Http;
using Microsoft.AspNet.SignalR.Client;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Events;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class ClientIssuePageViewModel : BindableBase, INavigatingAware
    {
        private Issue _issue;
        public Issue Issue
        {
            get { return _issue; }
            set { SetProperty(ref _issue, value); }
        }
        private Technicians technicians;
        public Technicians Technicians
        {
            get { return technicians; }
            set { SetProperty(ref technicians, value); }
        }
        private DateStamp dateStamp;
        public DateStamp DateStamp
        {
            get { return dateStamp; }
            set { SetProperty(ref dateStamp, value); }
        }
        private string tenantName;
        public string TenantName
        {
            get { return tenantName; }
            set { SetProperty(ref tenantName, value); }
        }
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;
        public ClientIssuePageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;

            DeleteCommand = new DelegateCommand(Delete);
            MarkCompleteCommand = new DelegateCommand(MarkComplete);
            ClientModule.hubProxy.On<Issue>("updateIssue", _issue =>
            {
                Issue = _issue;
            });

        }

        private async Task GetDate(string IssueId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading date");
                var date = await ServerPath.Path
                       .AppendPathSegment("/api/calendar/jobitem/" + ClientModule.Tenant + "/" + IssueId)
                       .GetJsonAsync<DateStamp>();
                if (date != null)
                    DateStamp = date;
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
        private async Task GetIssue(string IssueId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading issue");
                var _issue = await ServerPath.Path
                       .AppendPathSegment("/api/issues/getissue/" + IssueId + "/" + ClientModule.Tenant)
                       .GetJsonAsync<Issue>();
                if (_issue != null)
                {
                    Issue = _issue;
                    if(Issue.JobItem != null)
                    {
                        await GetTechnician(Issue.JobItem.JobCard.TechnicianId);
                    }
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
        private async Task GetTechnician(string TechnicianId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading Technician");
                var tech = await ServerPath.Path
                       .AppendPathSegment("/api/technicians/gettechnician/" +  ClientModule.Tenant + "/" + TechnicianId)
                       .GetJsonAsync<Technicians>();
                if (tech != null)
                    Technicians = tech;
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {

                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }

        private bool visibility;
        public bool Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
        }
        private async void MarkComplete()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Mark issue as complete?", "Complete", "Yes", "Cancel", null);
                if (result)
                {
                    Issue.IsResolved = true;
                    Issue.Status = "Resolved";
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating");
                    var complete = await ServerPath.Path
                        .AppendPathSegment("/api/issues/updateissue/" + TenantName)
                        .PutJsonAsync(Issue);
                    if (complete.StatusCode == HttpStatusCode.OK)
                    {
                        Issue = Issue;
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();

                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private async void Delete()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete issue?", "Delete", "Delete", "Cancel", null);
                if (result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Deleting...");
                    var delete = await ServerPath.Path
                        .AppendPathSegment("/api/issues/deleteissue/" + TenantName + "/" + Issue.IssueId)
                        .DeleteAsync();
                    if (delete.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        _eventAggregator.GetEvent<DeleteIssueEvent>().Publish(Issue);
                        await _navigationService.GoBackAsync();
                    }
                    else
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
                    }
                }
            }
            catch (Exception)
            {

                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed"); ;
            }
        }

        
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand MarkCompleteCommand { get; set; }
       
        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("issue"))
                {
                    var _issue = parameters["issue"] as Issue;
                    if (_issue != null)
                    {
                        await GetIssue(_issue.IssueId);
                        await GetDate(_issue.IssueId);
                    }

                    TenantName = parameters["tenantName"].ToString();
                    Visibility = !Issue.IsResolved;
                }
            }
        }

        
    }
}
