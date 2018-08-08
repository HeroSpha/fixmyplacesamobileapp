using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Helpers;
using SharedCode.Models;
using System.Collections.ObjectModel;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminPickIssuesViewModel : BindableBase
    {
        private ObservableCollection<Issue> issues;
        public ObservableCollection<Issue> Issues
        {
            get { return issues; }
            set { SetProperty(ref issues, value); }
        }

        private JobCard job;
        public JobCard Job
        {
            get { return job; }
            set { SetProperty(ref job, value); }
        }


        private IFlurlClient FlurlClient;
        public DelegateCommand GetTechnicianCommand { get; set; }
        INavigationService _navigationService;
        public AdminPickIssuesViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            
            _navigationService = navigationService;
            GetTechnicianCommand = new DelegateCommand(GetTechnician);
            GetIssues();
        }
        private async  void GetIssues()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getloggedissues/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken).GetJsonListAsync();
                if (issues != null)
                {
                    var loggedIssues = issues.Select(issue => new Issue
                    {
                        IssueId = issue.issueId,
                        Title = issue.title,
                        Category = new Category { CategoryName = issue.category.categoryName}
                    });
                    Issues = new ObservableCollection<Issue>(loggedIssues);
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }
        private async void GetTechnician()
        {
            if(Issues.Where(p => p.IsResolved == true).Count() > 0)
            {
                NavigationParameters para = new NavigationParameters();
                AdminModule.Issues = new List<Issue>(Issues.Where(p => p.IsResolved == true).ToList());
                para.Add("job", Job);
                await _navigationService.NavigateAsync("AdminAddTechnician", para);
            }
            else
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Selected at least one item.");
            }
            
        }

       
    }
}
