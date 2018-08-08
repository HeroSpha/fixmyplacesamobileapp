using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using System.Collections.ObjectModel;
using SharedCode.Models;
using SharedCode.Helpers;
using Prism.Navigation;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminAddTechnicianViewModel : BindableBase
    {
        public int Count { get; set; }
        public string Title { get; set; } = "Add Technician";
      
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
            set
            {
                SetProperty(ref technician, value);
                CanSave = Technician != null;
            }
        }
      
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand JobcardCommand { get; set; }
        INavigationService _navigationService;
        public AdminAddTechnicianViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Job = AdminModule.Jobcard;
            Issues = new ObservableCollection<Issue>(AdminModule.Issues);
            Count = Issues.Count;
            
            SaveCommand = new DelegateCommand(Save).ObservesCanExecute(() => CanSave);
            JobcardCommand = new DelegateCommand(GoBack);
            GetTechnicians();
            AddRange();
        }

        private async void GoBack()
        {
            await _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminJobcard", useModalNavigation:true);
        }

        private async void GetTechnicians()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var techs = await ServerPath.Path
                    .AppendPathSegment("/api/technicians/gettechnicians/" + AdminModule.TenantName).WithOAuthBearerToken(AdminModule.AccessToken).GetJsonListAsync();
                if (techs != null)
                {
                    var loggedIssues = techs.Select(tech => new Technicians
                    {
                        Name = tech.name,
                        TechnicianId = tech.technicianId,
                        Description = tech.description
                    });
                    Technicians = new ObservableCollection<Technicians>(loggedIssues);
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {
              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }
        private void AddRange()
        {
            AdminModule.hubProxy.On<JobCard>("addJobCard", async text =>
            {
                try
                {
                    var selected = Issues.Where(p => p.IsResolved == true).ToList();
                    List<JobItem> JobItems = new List<JobItem>();
                    foreach (var item in selected)
                    {
                        JobItems.Add(new JobItem { IssueId = item.IssueId, JobCardId = text.JobCardId, JobItemId = item.IssueId });
                    }
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating issues");
                    var result = await ServerPath.Path
                    .AppendPathSegment("/api/jobitems/addrange/" + AdminModule.TenantName).WithOAuthBearerToken(AdminModule.AccessToken).PostJsonAsync(JobItems);
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Issues = null;
                        Technician = null;
                        Technicians = null;
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Job card created.");
                    }
                    else
                    {
                        
                    }
                }
                catch (Exception)
                {

                   
                }
                finally
                {
                    try
                    {
                       
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }


            });
        }
        private async void Save()
        {
            try
            {
                if(Issues.Where(p => p.IsResolved == true).Count() > 0)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Saving...");
                    Job.Date = DateTime.Now;
                    Job.TechnicianId = Technician.TechnicianId;

                    var result = await ServerPath.Path
                        .AppendPathSegment("/api/jobcards/addjobcard/" + AdminModule.TenantName).WithOAuthBearerToken(AdminModule.AccessToken).PostJsonAsync(Job);
                    if (result != null)
                    {

                    }
                    
                }
                else
                {
                   await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Select atleast one issue.");
                }
            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

       

    }
}
