using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Models;
using Flurl.Http;
using System.Collections.ObjectModel;
using SharedCode.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminCostPageViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private Issue _issue;
        public Issue Issue
        {
            get { return _issue; }
            set { SetProperty(ref _issue, value); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private ObservableCollection<JobItemCost> _jobitemCosts;
        public ObservableCollection<JobItemCost> JobItemCosts
        {
            get { return _jobitemCosts; }
            set { SetProperty(ref _jobitemCosts, value); }
        }
        INavigationService _navigationService;
        public DelegateCommand AddCostCommand { get; set; }
        public DelegateCommand<JobItemCost> DeleteCost { get; set; }
        public AdminCostPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            AddCostCommand = new DelegateCommand(Add);
            DeleteCost = new DelegateCommand<JobItemCost>(Delete);
            AddCost();
        }

       
        private JobItemCost selectedCost;
        public JobItemCost SelectedCost
        {
            get { return selectedCost; }
            set
            {
                SetProperty(ref selectedCost, value);
                if (SelectedCost != null)
                {
                    Delete(SelectedCost);

                }
            }
        }
        private async void Add()
        {
            NavigationParameters para = new NavigationParameters();
            para.Add("issue", Issue);
            await _navigationService.NavigateAsync("AdminAddCostPage", para);
        }
        private void AddCost()
        {
            AdminModule.hubProxy.On<JobItemCost>("addJobItemCost", cost =>
            {
                JobItemCosts.Add(cost);
            });
        }
        private async void Delete(JobItemCost obj)
        {
            try
            {
                var delete = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete cost?", "Delete", "Delete", "Cancel", null);
                if (delete)
                {
                    var result = await ServerPath.Path
                        .AppendPathSegment("/api/itemcost/deletecost/" + AdminModule.TenantName + "/" + SelectedCost.CostId).WithOAuthBearerToken(AdminModule.AccessToken).DeleteAsync();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var item = JobItemCosts.Where(p => p.CostId == SelectedCost.CostId).FirstOrDefault();
                        if (item != null)
                        {
                            JobItemCosts.Remove(item);
                        }
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    }
                    else
                       await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to delete.");
                    SelectedCost = null;
                }
            }
            catch (Exception)
            {
                SelectedCost = null;
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to delete.");
            }
        }
        private async Task GetCosts()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading...");
                var providers = await ServerPath.Path
                    .AppendPathSegment( "/api/itemcost/getjobitemcost/" + AdminModule.TenantName + "/" + Issue.IssueId)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<JobItemCost>>();
                
                JobItemCosts = new ObservableCollection<JobItemCost>(providers);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("issue"))
                {
                    Issue = ((Issue)parameters["issue"]);
                    Title = Issue.Title;
                    await GetCosts();
                }
            }
        }
    }
}
