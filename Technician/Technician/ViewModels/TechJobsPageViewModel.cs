using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Flurl.Http;
using Prism.Navigation;
using SharedCode.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechJobsPageViewModel : BindableBase
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
                    _navigationService.NavigateAsync("TechJobItemsPage", para);
                    SelectedJob = null;
                }
            }
        }
        INavigationService _navigationService;
        public TechJobsPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            InfoCommand = new DelegateCommand<JobCard>(Details);
            GetJobCards().ConfigureAwait(true);
            TechnicianModule.hubProxy.On<JobCard>("deleteJobCard", _job =>
            {
                Acr.UserDialogs.UserDialogs.Instance.AlertAsync("done");
                var _item  = JobList.FirstOrDefault(p => p.JobCardId == _job.JobCardId);
                if(_item != null)
                {
                    JobList.Remove(_item);
                }
            });
        }
        private async Task GetJobCards()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading...");
                var jobs = await ServerPath.Path
                    .AppendPathSegment("/api/jobcards/gettechnicianjobcard/" + TechnicianModule.TenantName + "/" + TechnicianModule.UserId)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync<List<JobCard>>();
               
                JobList = new ObservableCollection<JobCard>(jobs);
                _jobs = new ObservableCollection<JobCard>(JobList);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.InnerException.Message) ;
            }

        }
        private async void Details(JobCard obj)
        {
            await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(obj.Description, "Job description");
        }
    }
}
