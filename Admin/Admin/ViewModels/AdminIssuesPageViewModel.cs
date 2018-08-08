using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Prism.Navigation;
using System.Threading.Tasks;
using Flurl.Http;
using SharedCode.Helpers;
using Prism.Events;
using SharedCode.Events;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminIssuesPageViewModel : BindableBase
    {
        private IFlurlClient FlurlClient { get; set; }

        private Issue selectedIssue;
        public Issue SelectedIssue
        {
            get { return selectedIssue; }
            set { SetProperty(ref selectedIssue, value);
                if(SelectedIssue != null)
                {
                    if(SelectedIssue.Latitude.HasValue)
                    {
                        AdminModule.Latitude = SelectedIssue.Latitude;
                        AdminModule.Longitude = SelectedIssue.Longitude;
                    }
                    else
                    {
                        AdminModule.Latitude = null;
                        AdminModule.Longitude = null;
                    }
                    NavigationParameters para = new NavigationParameters();
                    para.Add("issue", SelectedIssue);
                    _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminIssueDetailPage", para, useModalNavigation: true);
                    SelectedIssue = null;
                }
            }
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
                        var _filtered = MyIssues.Where(p => p.Title.ToLower().Contains(SearchKey.ToLower()) || p.Address.ToLower().Contains(SearchKey.ToLower()));
                        Issues = new ObservableCollection<Issue>(_filtered);
                    }
                    else
                    {
                        Issues = MyIssues;
                    }
                }
                catch (Exception)
                {


                }
            }
        }
        public ObservableCollection<Issue> MyIssues { get; set; }
        private ObservableCollection<Issue> issues;
        public ObservableCollection<Issue> Issues
        {
            get { return issues; }
            set { SetProperty(ref issues, value); }
        }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public AdminIssuesPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            GetIssues().ConfigureAwait(true);
            //Events
            _eventAggregator.GetEvent<DeleteIssueEvent>().Subscribe(DeleteIssue);
          
        }

        private void DeleteIssue(Issue obj)
        {
            Issues.Remove(obj);
        }

        private async Task GetIssues()
        {

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("loading");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getallissues/" + AdminModule.TenantName)
                .WithOAuthBearerToken(AdminModule.AccessToken).GetJsonListAsync();
                if (issues != null)
                {
                    var items = issues.Select(_issue => new Issue
                    {
                        IssueId = _issue.issueId,
                        Title = _issue.title,
                        Address = _issue.address,
                        Status = _issue.status,
                        Description = _issue.description,
                        PostedOn = _issue.postedOn,
                        Longitude = _issue.longitude,
                        Latitude = _issue.latitude,
                        CategoryId = _issue.categoryId,
                        CustomerId = _issue.customerId
                    });
                    Issues = new ObservableCollection<Issue>(items.OrderBy(p => p.PostedOn));
                    MyIssues = new ObservableCollection<Issue>(Issues);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error"); 
            }
        }
    }
}
