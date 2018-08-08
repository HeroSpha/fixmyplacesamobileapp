using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Navigation;
using SharedCode.Models;
using SharedCode.Helpers;
using Flurl.Http;
using Flurl;

namespace Client.ViewModels
{
    public class ClientSupportPageViewModel : BindableBase
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
        public ClientSupportPageViewModel(INavigationService navigationService)
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
            await _navigationService.NavigateAsync("PostPage");
        }

        private async Task GetCustomer()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("User information");
                var customer = await ServerPath.Path
                    .AppendPathSegment("/api/customers/getcustomer/" + ClientModule.UserId + "/FixmyPlace Support").WithOAuthBearerToken(ClientModule.AccessToken).GetJsonAsync();
                if (customer != null)
                {
                    var profile = new Customer
                    {
                        CustomerId = customer.customerId,
                        Firstname = customer.firstName,
                        Lastname = customer.lastName,
                        Email = customer.email,
                        Phone = customer.phone
                    };
                    Customer = profile;


                }
                else
                {
                    await _navigationService.NavigateAsync("ClientAddCustomer");
                }

            }
            catch (Exception ex)
            {

                if (Customer == null)
                {
                    await _navigationService.NavigateAsync("ClientAddCustomer");
                }
            }
        }

        private async Task LoadIssues()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading Issues");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getcustomerissues/FixmyPlace Support" + "/" + ClientModule.UserId)
                    .WithOAuthBearerToken(ClientModule.AccessToken).GetJsonListAsync();

                if (issues != null)
                {
                    Issues = issues.Select(issue => new Issue
                    {
                        IssueId = issue.issueId,
                        Title = issue.title,
                        Description = issue.description,
                        Address = issue.address,
                        Status = issue.status,
                        CategoryId = issue.categoryId,
                        DateResolved = issue.dateResolved,
                        ImageUrl1 = issue.imageUrl1,
                        ImageUrl2 = issue.imageUrl2,
                        ImageUrl3 = issue.imageUrl3,
                        IsResolved = issue.isResolved,
                        JobPerformed = issue.jobPerformed,
                        PostedOn = issue.postedOn
                    }).ToList();
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No Issue found");
            }
        }
    }
}
