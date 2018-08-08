using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Admin.ViewModels
{
	public class AdminPicTechnicinPageViewModel : BindableBase, INavigatingAware
	{
        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetProperty(ref canSave, value); }
        }
        public string IssueId { get; set; }
        public Customer Customer { get; set; }
        private Technicians technician;
        public Technicians Technician
        {
            get { return technician; }
            set { SetProperty(ref technician, value);
                if(Technician != null)
                {
                    CanSave = true;
                }
                else
                {
                    CanSave = false;
                }
            }
        }
        private ObservableCollection<Technicians> technicians;
        public ObservableCollection<Technicians> Technicians
        {
            get { return technicians; }
            set { SetProperty(ref technicians, value); }
        }
        INavigationService _navigationService;
        public DelegateCommand NextCommand { get; set; }
        public AdminPicTechnicinPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NextCommand = new DelegateCommand(Next).ObservesCanExecute(() => CanSave);
            GetTechnicians();
        }

        private async void Next()
        {
            NavigationParameters nav = new NavigationParameters
            {
                {"issueId", IssueId },
                {"technicianId", Technician.TechnicianId },
                {"customer", Customer }
            };
            await _navigationService.NavigateAsync("AddJobcardPage", nav);
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
            catch
            {

            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                IssueId = parameters["issueId"].ToString();
                Customer = parameters["customer"] as Customer;
            }
        }
    }
}
