using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using System.Threading.Tasks;
using SharedCode.Models;
using Flurl.Http;
using SharedCode.Helpers;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminSupportPageViewModel : BindableBase
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
        public AdminSupportPageViewModel(INavigationService navigationService)
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
            await _navigationService.NavigateAsync("AdminPostPage");
        }

        private async Task GetCustomer()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("User information");
                var customer = await ServerPath.Path
                    .AppendPathSegment("/api/customers/getcustomer/" + AdminModule.UserId + "/FixmyPlace Support" )
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<Customer>();
                if (customer != null)
                {
                   
                    Customer = customer;

                   
                }
                else
                {
                    await _navigationService.NavigateAsync("AdminRegisterPage");
                }

            }
            catch (Exception ex)
            {

               if(Customer == null)
                {
                    await _navigationService.NavigateAsync("RegisterPage");
                }
            }
        }
     
        private async Task LoadIssues()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading Issues");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getcustomerissues/FixmyPlace Support"  + "/" + AdminModule.UserId)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
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
