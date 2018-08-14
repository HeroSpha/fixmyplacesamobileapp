using Flurl;
using Flurl.Http;
using Microsoft.AspNet.SignalR.Client;
using Plugin.ExternalMaps;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Models;
using SharedCode.Events;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.ViewModels
{
	public class AdminIssueDetailPageViewModel : BindableBase, INavigatingAware
    {

        private Technicians technicians;
        public Technicians Technicians
        {
            get { return technicians; }
            set { SetProperty(ref technicians, value); }
        }
        private IFlurlClient FlurlClient;
        private Issue issue;
        public Issue Issue
        {
            get { return issue; }
            set { SetProperty(ref issue, value); }
        }
        private bool publishVisible;
        public bool PublishVisible
        {
            get { return publishVisible; }
            set { SetProperty(ref publishVisible, value); }
        }
        private bool viewVisible;
        public bool ViewVisible
        {
            get { return viewVisible; }
            set { SetProperty(ref viewVisible, value); }
        }
        private bool isCost;
        public bool IsCost
        {
            get { return isCost; }
            set { SetProperty(ref isCost, value); }
        }
        private bool locationAvailable = false;
        public bool LocationAvailable
        {
            get { return locationAvailable; }
            set { SetProperty(ref locationAvailable, value); }
        }

        private bool isResolved = false;
        public bool IsResolved
        {
            get { return isResolved; }
            set { SetProperty(ref isResolved, value); }
        }
        public ObservableCollection<Quotation> _quotations { get; set; }

        private string searchkey;
        public string SearchKey
        {
            get { return searchkey; }
            set { SetProperty(ref searchkey, value);
                if(!string.IsNullOrEmpty(SearchKey))
                {
                    var temp = _quotations.Where(p => p.Description.ToLower().Contains(SearchKey) || p.Technicians.Name.ToLower().Contains(SearchKey)).ToList();
                    Quotations = new ObservableCollection<Quotation>(temp);
                }
                else
                {
                    Quotations = _quotations;
                }
            }
        }
        private bool isVisible = false;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }
        private DateStamp dateStamp;
        public DateStamp DateStamp
        {
            get { return dateStamp; }
            set { SetProperty(ref dateStamp, value); }
        }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand CompleteCommand { get; set; }
        public DelegateCommand LocationCommand { get; set; }
        public DelegateCommand AddCostCommand { get; set; }
        public DelegateCommand PublishCommand { get; set; }
        public DelegateCommand ImagesCommand { get; set; }
        public DelegateCommand JobcardCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public AdminIssueDetailPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            IsCost = false;
            PublishCommand = new DelegateCommand(Publish);
           
            if (AdminModule.Latitude == null)
            {
                LocationAvailable = false;
            }
            else
            {
                LocationAvailable = true;
            }
            _eventAggregator = eventAggregator;
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            JobcardCommand = new DelegateCommand(Jobcard);
            DeleteCommand = new DelegateCommand(DeleleIssue);
            CompleteCommand = new DelegateCommand(Complete);
            AddCostCommand = new DelegateCommand(AddCost);
            LocationCommand = new DelegateCommand(ViewLocation);
            ImagesCommand = new DelegateCommand(Images);
            AdminModule.hubProxy.On<Issue>("updateIssue", _issue =>
            {
                Issue = _issue;
            });
        }
        private bool isAck;
        public bool IsAck
        {
            get { return isAck; }
            set { SetProperty(ref isAck, value); }
        }
        private async void Jobcard()
        {
            NavigationParameters nav = new NavigationParameters
            {
                {"issueId", Issue.IssueId },
                {"customer", Issue.Customer }
            };
            await _navigationService.NavigateAsync("AdminPicTechnicinPage", nav);
        }

        private async void Images()
        {
            NavigationParameters para = new NavigationParameters
            {
                {"issue", Issue }
            };
            await _navigationService.NavigateAsync("ImagesPage", para);
        }

        private async Task GetDate(string IssueId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var date = await ServerPath.Path
                       .AppendPathSegment("/api/calendar/jobitem/" + AdminModule.TenantName + "/" + IssueId)
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


        private async void Publish()
        {
            var publish = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Publish issue for quotation?", "", "Publish", "Cancel", null);
            if (publish)
            {
                issue.Status = "quotation";
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Publishing");
                var updated = await ServerPath.Path
                    .AppendPathSegment("/api/issues/updateissue/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .PutJsonAsync(Issue);
                if (updated != null)
                {
                    PublishVisible = false;
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                else
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to publish");
            }
        }
        private async Task GetIssue(string IssueId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var _issue = await ServerPath.Path
                       .AppendPathSegment("/api/issues/getissue/" + IssueId + "/" + AdminModule.TenantName)
                       .GetJsonAsync<Issue>();
                if (_issue != null)
                {
                    Issue = _issue;
                    if (Issue.JobItem != null)
                    {
                        IsCost = true;
                        await GetDate(_issue.IssueId);
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
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var tech = await ServerPath.Path
                       .AppendPathSegment("/api/technicians/gettechnician/" + AdminModule.TenantName + "/" + TechnicianId)
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
        private async void ViewLocation()
        {
            await CrossExternalMaps.Current.NavigateTo("Issue location", double.Parse(Issue.Latitude.ToString()), double.Parse(Issue.Longitude.ToString()));
        }

        private async void AddCost()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("issue", Issue);
            await _navigationService.NavigateAsync("AdminCostPage", para);
        }

        private async void Complete()
        {
            try
            {
                var jobperformed = await Acr.UserDialogs.UserDialogs.Instance.PromptAsync("", "Job Performed", "Mark as complete", "Cancel", "Describe job performed", Acr.UserDialogs.InputType.Default);
                if (jobperformed.Ok)
                {
                    Issue.JobPerformed = jobperformed.Value;
                    Issue.IsResolved = true;
                    Issue.Status = "Resolved";

                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating");
                    var updateResult = await ServerPath.Path
                        .AppendPathSegment("/api/issues/updateissue/" + AdminModule.TenantName)
                        .WithOAuthBearerToken(AdminModule.AccessToken).PutJsonAsync(Issue);
                    if (updateResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        IsResolved = false;
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error");
                    }
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to update issue.");
            }
        }
        private ObservableCollection<Quotation> quotations;
        public ObservableCollection<Quotation> Quotations
        {
            get { return quotations; }
            set { SetProperty(ref quotations, value); }
        }
        private string issueId;
        public string IssueId
        {
            get { return issueId; }
            set { SetProperty(ref issueId, value); }
        }
        private async void DeleleIssue()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete issue?", "Delete", "Delete", "Cancel", null);
                if (result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Deleting...");
                    var delete = await ServerPath.Path
                        .AppendPathSegment("/api/issues/deleteissue/" + AdminModule.TenantName + "/" + Issue.IssueId).DeleteAsync();
                    if (delete.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        _eventAggregator.GetEvent<DeleteIssueEvent>().Publish(Issue);
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
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
        private async Task GetQuotations(string IssueId)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var quotations = await ServerPath.Path
                    .AppendPathSegment("/api/quotations/getquotation/" + IssueId + "/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<Quotation>>();
                Quotations = new ObservableCollection<Quotation>(quotations);
                _quotations = Quotations;
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("");
                throw;
            }
        }
        private Quotation quotation;
        public Quotation Quotation
        {
            get { return quotation; }
            set { SetProperty(ref quotation, value);
                if(Quotation != null)
                {
                    
                   CreateJobCard(Quotation.TechnicianId);
                   Quotation = null;
                }
            }
        }

        private async void CreateJobCard(string technicianId)
        {
            try
            {
                if(Issue.Status == "Ack" || Issue.Status == "quotation")
                {
                    var confirm = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Create jobcard?", "Jobcard", "Create", "Cancel", null);
                    if(confirm)
                    {
                        NavigationParameters nav = new NavigationParameters
                        {
                            {"issueId", Issue.IssueId },
                            {"technicianId", technicianId },
                            {"customer", Issue.Customer }
                           
                        };
                        await _navigationService.NavigateAsync("AddJobcardPage", nav);
                    }
                }
                else
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Jobcard cannot be created for this issue.");
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("failed to create jobcard");
            }
        }

        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                var _issue = parameters["issue"] as Issue;
                if(_issue != null)
                {
                    await GetIssue(_issue.IssueId);
                    
                    if (_issue.Status == "quotation")
                    {
                        await GetQuotations(_issue.IssueId);
                    }
                }
             
                switch (Issue.Status)
                {
                    case "Ack":
                        {
                            IsAck = true;
                            IsVisible = false;
                            IsResolved = true;
                            PublishVisible = true;
                            ViewVisible = false;
                        }
                        break;
                    case "Pending":
                        {
                            IsAck = false;
                            IsVisible = true;
                            IsResolved = true;
                            PublishVisible = false;
                            ViewVisible = true;
                        }
                        break;
                    case "Resolved":
                        {
                            IsAck = false;
                            IsVisible = true;
                            IsResolved = false;
                            PublishVisible = false;
                            ViewVisible = true;
                        }
                        break;
                    case "quotation":
                        {
                            IsAck = false;
                            PublishVisible = false;
                            ViewVisible = true;
                        }
                        break;

                }
            }
        }
      
	}
}
