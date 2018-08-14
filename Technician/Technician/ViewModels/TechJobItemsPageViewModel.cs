using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Prism.Navigation;
using System.Threading.Tasks;
using SharedCode.Helpers;
using SharedCode.Models;
using Microsoft.AspNet.SignalR.Client;
using Flurl.Http;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechJobItemsPageViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string searchKey;
        public string SearchKey
        {
            get { return searchKey; }
            set
            {
                SetProperty(ref searchKey, value);
                try
                {
                    if (SearchKey != null)
                    {
                        var _filtered = jobItems.Where(p => p.Title.ToLower().Contains(SearchKey.ToLower()));
                        JobItems = new ObservableCollection<Issue>(_filtered);
                    }
                    else
                    {
                        JobItems = jobItems;
                    }
                }
                catch (Exception)
                {


                }
            }
        }
        private JobCard _job;
        public JobCard Job
        {
            get { return _job; }
            set { SetProperty(ref _job, value); }
        }
        private ObservableCollection<Issue> _jobitems;
        public ObservableCollection<Issue> JobItems
        {
            get { return _jobitems; }
            set { SetProperty(ref _jobitems, value); }
        }
        private ObservableCollection<Issue> jobItems;

        private Issue _selectedIssue;
        public Issue SelectedIssue
        {
            get { return _selectedIssue; }
            set
            {
                SetProperty(ref _selectedIssue, value);
                if (SelectedIssue != null)
                {
                    if(SelectedIssue.Latitude != null)
                    {
                        TechnicianModule.Latitude = SelectedIssue.Latitude;
                        TechnicianModule.Longitude = SelectedIssue.Longitude;
                    }
                    else
                    {
                        TechnicianModule.Latitude = null;
                        TechnicianModule.Longitude = null;
                    }
                    NavigationParameters para = new NavigationParameters();

                    para.Add("jobitem", SelectedIssue);
                    _navigatiponService.NavigateAsync("TechJobItem", para);
                    SelectedIssue = null;
                }
            }
        }
        INavigationService _navigatiponService;
        public TechJobItemsPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigatiponService = navigationService;
            MenuCommand = new DelegateCommand<Issue>(menu);
            UpdateIssie();

        }

        private void UpdateIssue(Issue obj)
        {
            var found = JobItems.FirstOrDefault(p => p.IssueId == obj.IssueId);
            found.IsResolved = true;
            found.JobPerformed = obj.JobPerformed;
        }

        private async void menu(Issue issue)
        {
            string result = null;
            switch (issue.IsResolved)
            {

                case true:
                    {
                        result = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("menu", "Cancel", "", null, "Add Cost", "Schedule");
                        await ProcessMenu(result, issue);
                    }
                    break;
                case false:
                    {
                        result = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("menu", "Cancel", "", null, "Mark as complete", "Add Cost", "Schedule");
                        await ProcessMenu(result, issue);
                    }
                    break;
            }

        }
        private async Task ProcessMenu(string result, Issue issue)
        {
            NavigationParameters para = null;
            switch (result)
            {
                case "Add Cost":
                    {
                        para = new NavigationParameters();
                        para.Add("jobitem", issue);
                        await _navigatiponService.NavigateAsync("TechCostPage", para);
                    }
                    break;
                case "Schedule":
                    {
                        para = new NavigationParameters();
                        para.Add("jobitem", issue);
                        await _navigatiponService.NavigateAsync("TechAddDate", para);

                        //Updating the item
                    }
                    break;
                case "Mark as complete":
                    {
                        try
                        {
                            var jobperformed = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("", "Job Performed", "Mark as complete", "Cancel", "Describe job performed", Acr.UserDialogs.InputType.Default);
                            if (jobperformed.Ok)
                            {
                                issue.JobPerformed = jobperformed.Value;
                                issue.IsResolved = true;
                                issue.Status = "Resolved";
                                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating...");
                                var updateResult = await ServerPath.Path
                                    .AppendPathSegment("/api/issues/updateissue/" + TechnicianModule.TenantName)
                                    .WithOAuthBearerToken(TechnicianModule.AccessToken).PutJsonAsync(issue);
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
                        catch (Exception)
                        {


                        }
                    }
                    break;
            }
        }
        public DelegateCommand<Issue> MenuCommand { get; set; }
        private async Task GetJobcardItems()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading...");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/jobitems/alljobitems/" + TechnicianModule.TenantName + "/" + Job.JobCardId)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync<List<Issue>>();
                JobItems = new ObservableCollection<Issue>(issues);
                jobItems = new ObservableCollection<Issue>(JobItems);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void UpdateIssie()
        {
            TechnicianModule.hubProxy.On<Issue>("updateIssue", issue =>
            {
               
                var updateItem = JobItems.FirstOrDefault(p => p.IssueId == issue.IssueId);
                if(updateItem != null)
                {
                    updateItem.IsResolved = true;
                    updateItem.Status = "Resolved";
                    updateItem.JobPerformed = issue.JobPerformed;
                }

            });
        }
        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("job"))
                {
                    Job = ((JobCard)parameters["job"]);
                    Title = Job.Name;
                     await GetJobcardItems();
                }
            }
        }
    }
}
