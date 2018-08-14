using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using SharedCode.Models;
using Flurl.Http;
using SharedCode.Helpers;
using Prism.Navigation;
using System.Threading.Tasks;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminSelectTechniciansPageViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
       
        private ObservableCollection<Technicians> technicians;
        public ObservableCollection<Technicians> Technicians
        {
            get { return technicians; }
            set { SetProperty(ref technicians, value); }
        }
        private JobCard job;
        public JobCard Job
        {
            get { return job; }
            set { SetProperty(ref job, value); }
        }
        private bool cansave;
        public bool CanSave
        {
            get { return cansave; }
            set { SetProperty(ref cansave, value); }
        }
        private ObservableCollection<Issue> issues;
        public ObservableCollection<Issue> Issues
        {
            get { return issues; }
            set { SetProperty(ref issues, value); }
        }
        private Technicians technician;
        public Technicians Technician
        {
            get { return technician; }
            set { SetProperty(ref technician, value);
                CanSave = Technician != null;
            }
        }
        public DelegateCommand SaveJobCommand { get; set; }
        public AdminSelectTechniciansPageViewModel()
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            GetTechnicians().ConfigureAwait(true);

        }

        private async void Save()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Saving...");
                Job.Date = DateTime.Now;
                Job.TechnicianId = Technician.TechnicianId;

                var result = await ServerPath.Path
                    .AppendPathSegment("/api/jobcards/addjobcard").WithOAuthBearerToken(AdminModule.AccessToken).PostJsonAsync(Job);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

        private async Task GetTechnicians()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var techs = await ServerPath.Path
                    .AppendPathSegment("/api/technicians/gettechnicians/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<Technicians>>();
                Technicians = new ObservableCollection<Technicians>(techs);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                Job = parameters["job"] as JobCard;
                var items = parameters["issues"] as List<Issue>;
                Issues = new ObservableCollection<Issue>(items);
            }
        }
    }
}
