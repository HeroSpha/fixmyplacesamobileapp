using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.ViewModels
{
	public class AddJobcardPageViewModel : BindableBase, INavigatingAware
	{
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
        public string IssueId { get; set; }
        public string TechnicianId { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value);
                CanPost = !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Name);
            }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value);
                CanPost = !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Name);
            }
        }
        private bool canPost;
        public bool CanPost
        {
            get { return canPost; }
            set { SetProperty(ref canPost, value); }
        }
        INavigationService _navigationService;
        public AddJobcardPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CreatejobCommand = new DelegateCommand(CreateJob).ObservesCanExecute(() => CanPost);
        }
        private async Task JobItems(List<JobItem> items)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.Loading("Updating Items");
                var _items = await ServerPath.Path
                      .AppendPathSegment("/api/jobitems/addrange/" + AdminModule.TenantName)
                      .WithOAuthBearerToken(AdminModule.AccessToken)
                      .PostJsonAsync(items);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                if (_items.IsSuccessStatusCode)
                {
                    var customers = new List<Customer>();
                    customers.Add(Customer);
                    await UpdateCustomers(customers);
                }
               
            }
            catch (Exception)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to create jobcard");
            }
        }
        private async Task UpdateCustomers(List<Customer> customers)
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.Loading("Updating tenants");
                var _customers = await ServerPath.Path
                      .AppendPathSegment("/api/jobitems/updatecustomer/" + AdminModule.TenantName)
                      .WithOAuthBearerToken(AdminModule.AccessToken)
                      .PostJsonAsync(customers);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                if (_customers.IsSuccessStatusCode)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Jobcard created.");
                    await _navigationService.NavigateAsync("AdminJobcard");
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to create jobcard");
                    await _navigationService.NavigateAsync("AdminJobcard");
                }
                
            }
            catch (Exception)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
        private async void CreateJob()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.Loading("Creating jobcard");
                var job = await ServerPath.Path
                   .AppendPathSegment("/api/jobcards/addjobcard/" + AdminModule.TenantName)
                   .WithOAuthBearerToken(AdminModule.AccessToken)
                   .PostJsonAsync(
                    new
                    {
                        Name,
                        Description,
                        Date = DateTime.Now,
                        TechnicianId
                    }).ReceiveJson<JobCard>();
               
                if (job != null)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Customer.CustomerId = job.JobCardId;
                    var items = new List<JobItem>
                    {
                        new JobItem{ IssueId = IssueId, JobCardId = job.JobCardId, JobItemId = IssueId}
                    };

                     await JobItems(items);
                }
                else
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to create jobcard");
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to create jobcard");
            }
        }

        public DelegateCommand CreatejobCommand { get; set; }
        public void OnNavigatingTo(INavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                IssueId = parameters["issueId"].ToString();
                TechnicianId = parameters["technicianId"].ToString();
                Customer = parameters["customer"] as Customer;
            }
        }
    }
}
