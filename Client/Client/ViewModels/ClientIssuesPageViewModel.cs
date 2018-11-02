using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Prism.Navigation;
using SharedCode.Helpers;
using Flurl.Http;
using System.Threading.Tasks;
using Prism.Events;
using SharedCode.Events;
using System.Collections.ObjectModel;
using Flurl;
using Microsoft.AspNet.SignalR.Client;

namespace Client.ViewModels
{
    public class ClientIssuesPageViewModel : BindableBase, INavigatingAware
    {
        private string tenantName;
        public string TenantName
        {
            get { return tenantName; }
            set { SetProperty(ref tenantName, value); }
        }
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
        private IFlurlClient FlurlClient;
        private Issue _selectedIssue;
        public Issue SelectedIssue
        {
            get { return _selectedIssue; }
            set
            {
                SetProperty(ref _selectedIssue, value);
                if (SelectedIssue != null)
                {
                    NavigationParameters para = new NavigationParameters();
                    para.Add("issue", SelectedIssue);
                    para.Add("tenantName", TenantName);
                    _navigationService.NavigateAsync("ClientIssuePage", para);
                    SelectedIssue = null;
                }
            }
        }
        public DelegateCommand PostIssueCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public ClientIssuesPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            
             _navigationService = navigationService;
            PostIssueCommand = new DelegateCommand(Post);
             LoadIssues().ConfigureAwait(true);
            _eventAggregator.GetEvent<DeleteIssueEvent>().Subscribe(DeleteIssue);
            ClientModule.hubProxy.On<Issue>("updateIssue", _issue =>
            {
                var item = Issues.FirstOrDefault(p => p.IssueId == _issue.IssueId);
                var i = Issues.IndexOf(item);
                item.Status = "Resolved";
                Issues[i] = item;

            });
        }

        private void DeleteIssue(Issue obj)
        {
            Issues.Remove(obj);
        }

        private void Update(Issue obj)
        {
            var updateItem = Issues.Where(p => p.IssueId == obj.IssueId).FirstOrDefault();
            updateItem.Status = "Resolved";
        }

      
        private async Task LoadIssues()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading Issues");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getcustomerissues/" + ClientModule.Tenant + "/" + ClientModule.UserId)
                    .GetJsonAsync<List<Issue>>();

                Issues = new ObservableCollection<Issue>(issues);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No Issue found");
            }
        }
        private ObservableCollection<Issue> _issues;
        public ObservableCollection<Issue> Issues
        {
            get { return _issues; }
            set { SetProperty(ref _issues, value); }
        }

        private async void Post()
        {
            await _navigationService.NavigateAsync("ClientPostPage");
        }

        public  void OnNavigatingTo(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Customer = parameters["customer"] as Customer;
                TenantName = parameters["tenantName"].ToString();
                
            }
        }
    }
}
