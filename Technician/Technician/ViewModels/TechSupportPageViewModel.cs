using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using System.Threading.Tasks;
using SharedCode.Models;
using SharedCode.Helpers;
using Flurl.Http;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechSupportPageViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
        private List<Issue> _issues;
        public List<Issue> Issues
        {
            get { return _issues; }
            set { SetProperty(ref _issues, value); }
        }
        INavigationService _navigationService;
        public DelegateCommand AddIssueCommand { get; set; }
        public TechSupportPageViewModel(INavigationService navigationService)
        {
            AddIssueCommand = new DelegateCommand(AddIssue);
            _navigationService = navigationService;
            FlurlClient = new FlurlClient(ServerPath.Path);

            GetCustomer().ContinueWith(async _co =>
            {
                await LoadIssues();
            });

        }

        private async void AddIssue()
        {
            await _navigationService.NavigateAsync("TechPostPage");
        }

        private async Task GetCustomer()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("User information");
                var customer = await ServerPath.Path
                    .AppendPathSegment("/api/customers/getcustomer/" + TechnicianModule.UserId + "/FixmyPlace Support")
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync<Customer>();
                if (customer != null)
                {
                    
                    Customer = customer;


                }
                else
                {
                    await _navigationService.NavigateAsync("TechAddCustomer");
                }

            }
            catch (Exception ex)
            {

                if (Customer == null)
                {
                    await _navigationService.NavigateAsync("TechAddCustomer");
                }
            }
        }

        private async Task LoadIssues()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading Issues");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getcustomerissues/FixmyPlace Support" + "/" + TechnicianModule.UserId)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonAsync<List<Issue>>();

                Issues = new List<Issue>(issues);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No Issue found");
            }
        }
    }
}
