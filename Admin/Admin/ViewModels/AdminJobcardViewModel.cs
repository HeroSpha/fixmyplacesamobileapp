using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using System.Collections.ObjectModel;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Helpers;
using System.Threading.Tasks;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminJobcardViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;
        private ObservableCollection<JobCard> _joblist;
        public ObservableCollection<JobCard> JobList
        {
            get { return _joblist; }
            set { SetProperty(ref _joblist, value); }
        }
        private ObservableCollection<JobCard> _jobs;
        private string _seachKey;
        public string SearchKey
        {
            get { return _seachKey; }
            set
            {
                SetProperty(ref _seachKey, value);
                if (SearchKey != null)
                {
                    var _job = _jobs.Where(p => p.Name.ToLower().Contains(SearchKey.ToLower()));
                    JobList = new ObservableCollection<JobCard>(_job);

                }
                else
                {
                    JobList = _jobs;
                }
            }
        }
        public DelegateCommand<JobCard> InfoCommand { get; set; }
        private JobCard _selectedJob;
        public JobCard SelectedJob
        {
            get { return _selectedJob; }
            set
            {
                SetProperty(ref _selectedJob, value);
                if (SelectedJob != null)
                {
                    NavigationParameters para = new NavigationParameters();
                    para.Add("job", SelectedJob);
                    _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminJobcardItems", para, useModalNavigation:true);
                    SelectedJob = null;
                }
            }
        }
        public DelegateCommand AddJobCardCommand { get; set; }
        public DelegateCommand GetIssuesCommand { get; set; }
        INavigationService _navigationService;
        public AdminJobcardViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            InfoCommand = new DelegateCommand<JobCard>(Details);
            AddJobCardCommand = new DelegateCommand(AddJobcard);
            GetIssuesCommand = new DelegateCommand(GetIssues);
            GetJobCards().ConfigureAwait(true);
        }

        private async void GetIssues()
        {
            await _navigationService.NavigateAsync("AdminPickIssues");
        }

        private async void AddJobcard()
        {
            await _navigationService.NavigateAsync("AdminAddJobcardPage");
        }

        private async Task GetJobCards()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading...");
                var jobs = await ServerPath.Path
                    .AppendPathSegment("/api/jobcards/getjobcards/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<JobCard>>();
                JobList = new ObservableCollection<JobCard>(jobs);
                _jobs = new ObservableCollection<JobCard>(JobList);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.InnerException.Message);
            }

        }
        private async void Details(JobCard obj)
        {
            await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(obj.Description, "Job description");
        }
     
    }
}
