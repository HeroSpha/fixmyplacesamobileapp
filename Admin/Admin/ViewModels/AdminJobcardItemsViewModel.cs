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
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminJobcardItemsViewModel : BindableBase, INavigatingAware
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
                    GetOptions(SelectedIssue);
                }
            }
        }
        private async void GetOptions(Issue issue)
        {
            var action = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("Issue", "Cancel", "", null, "Details", "Remove");
            switch(action)
            {
                case "Details":
                    {
                        NavigationParameters para = new NavigationParameters();
                        para.Add("issue", SelectedIssue);
                       await _navigatiponService.NavigateAsync("AdminMasterPage/NavigationPage/AdminIssueDetailPage", para, useModalNavigation: true);
                        SelectedIssue = null;
                    }
                    break;
                case "Remove":
                    {
                        var confirm = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove issue", "", "Remove", "Cancel", null);
                        if(confirm)
                        {
                            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Deleting");
                            var deleted = await ServerPath.Path
                            .AppendPathSegment("/api/jobitems/deletejobitem/" + SelectedIssue.IssueId + "/" + AdminModule.TenantName).DeleteAsync();
                            if (deleted.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                                var item = JobItems.FirstOrDefault(p => p.IssueId == SelectedIssue.IssueId);
                                int i = JobItems.IndexOf(item);
                                JobItems.RemoveAt(i);
                                SelectedIssue = null;
                            }
                        }
                    }
                    break;
            }
            SelectedIssue = null;
        }
        INavigationService _navigatiponService;
        public DelegateCommand DeleteCommand { get; set; }
        public AdminJobcardItemsViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigatiponService = navigationService;
            MenuCommand = new DelegateCommand<Issue>(menu);
            DeleteCommand = new DelegateCommand(DeleteIssue);

            AdminModule.hubProxy.On<Issue>("updateIssue", text =>
            {
                var _issue = JobItems.FirstOrDefault(p => p.IssueId == text.IssueId);
                if (_issue != null)
                {
                    _issue.IsResolved = true;
                    _issue.Status = "Solved";
                }
            });
        }

        private async void DeleteIssue()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete this jobcard?", "", "Delete", "Cancel", null);
                if(result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Deleting");
                    var deleted = await ServerPath.Path
                        .AppendPathSegment("/api/jobcards/deletejobcard/" + Job.JobCardId + "/" + AdminModule.TenantName)
                        .WithOAuthBearerToken(AdminModule.AccessToken)
                        .DeleteAsync();
                    if(deleted.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await  _navigatiponService.NavigateAsync("AdminJobcard");
                    }
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error occured");
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to delete, try again later.");
            }
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
                        await _navigatiponService.NavigateAsync("AdminCostPage", para);
                    }
                    break;
                case "Schedule":
                    {
                        para = new NavigationParameters();
                        para.Add("jobitem", issue);
                        await _navigatiponService.NavigateAsync("AdminAddDate", para);

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
                                    .AppendPathSegment("/api/issues/updateissue/" + AdminModule.TenantName).WithOAuthBearerToken(AdminModule.AccessToken).PutJsonAsync(issue);
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
                    .AppendPathSegment("/api/jobitems/alljobitems/" + AdminModule.TenantName + "/" + Job.JobCardId)
                    .WithOAuthBearerToken(AdminModule.AccessToken).GetJsonListAsync();
                if (issues != null)
                {
                    var jobcards = issues.Select(issue => new Issue
                    {
                        IssueId = issue.issueId,
                        Title = issue.title,
                        Description = issue.description,
                        IsResolved = issue.isResolved,
                        Address = issue.address,
                        CustomerId = issue.customerId,
                        CategoryId = issue.categoryId,
                        JobPerformed = issue.jobPerformed,
                        Status = issue.status,
                        ImageUrl1 = issue.imageUrl1,
                        ImageUrl2 = issue.imageUrl2,
                        ImageUrl3 = issue.imageUrl3,
                        PostedOn = issue.postedOn,
                        DateResolved = issue.dateResolved,
                        Customer = new Customer { CustomerId = issue.customer.customerId, Email = issue.customer.email, Firstname = issue.customer.firstname, Lastname = issue.customer.lastname, Phone = issue.customer.phone },
                        Category = new Category { CategoryId = issue.category.categoryId, CategoryName = issue.category.categoryName }
                    });
                    JobItems = new ObservableCollection<Issue>(jobcards);
                    jobItems = new ObservableCollection<Issue>(JobItems);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }

            }
            catch (Exception ex)
            {

                throw;
            }
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
