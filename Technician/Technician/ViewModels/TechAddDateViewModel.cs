using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Flurl.Http;
using SharedCode.Helpers;
using Prism.Navigation;
using Flurl;

namespace TechTechnician.ViewModels
{
	public class TechAddDateViewModel : BindableBase, INavigatingAware
	{
        private IFlurlClient FlurlClient;
        private Issue _issue;
        public Issue Issue
        {
            get { return _issue; }
            set { SetProperty(ref _issue, value); }
        }
        private bool isUpdating = false;
        public bool IsUpdating
        {
            get { return isUpdating; }
            set { SetProperty(ref isUpdating, value); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public string DateStampId { get; set; }
        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }
        private DateTime _startDate = DateTime.Today;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }
      
        private DateTime _endDate = DateTime.Today;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }
        public DelegateCommand SaveCommand { get; set; }
       
        public TechAddDateViewModel()
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
           
            SaveCommand = new DelegateCommand(Save);
        }

        private async void Save()
        {
            try
            {
                if (!IsUpdating)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Posting date...");
                    var result = await ServerPath.Path
                        .AppendPathSegment("/api/calendar/adddate/" + TechnicianModule.TenantName).WithOAuthBearerToken(TechnicianModule.AccessToken).PostJsonAsync(new
                    {
                        Title = Header,
                        JobItemId = Issue.IssueId,
                        StartDate,
                        EndDate
                    });
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Date added.");
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        Header = string.Empty;
                        StartDate = DateTime.Now;
                        EndDate = DateTime.Now;
                    }
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating date...");
                    var result = await ServerPath.Path
                        .AppendPathSegment("/api/calendar/update/" + TechnicianModule.TenantName).WithOAuthBearerToken(TechnicianModule.AccessToken).PutJsonAsync(new
                    {
                        DateStampId,
                        Title = Header,
                        StartDate,
                        EndDate
                    });
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Date updated.");
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        Header = string.Empty;
                       
                        StartDate = DateTime.Now;
                        EndDate = DateTime.Now;
                       
                    }
                }
            }
            catch (FlurlHttpException ex)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Date already set.");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("jobitem"))
                {
                    Issue = ((Issue)parameters["jobitem"]);
                    Title = Issue.Title;
                }
                if (parameters.ContainsKey("update"))
                {
                    IsUpdating = true;
                    var Stamp = ((DateStamp)parameters["update"]);
                    Title = "Update date";
                    Header = Stamp.Title;
                    StartDate = Stamp.StartDate;
                    EndDate = Stamp.EndDate.Value;
                  
                    DateStampId = Stamp.DateStampId;

                    Issue = new Issue
                    {
                        IssueId = Stamp.JobItemId
                    };

                }
            }
        }
    }
}
