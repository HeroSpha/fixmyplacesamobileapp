using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Prism.Navigation;
using SharedCode.Helpers;
using Flurl.Http;
using Flurl;

namespace TechTechnician.ViewModels
{
	public class TechAddCostPageViewModel : BindableBase, INavigatingAware
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
        private decimal _price;
        public decimal Price
        {
            get { return _price; }
            set
            {
                SetProperty(ref _price, value);
                CanSave = !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Price.ToString());
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                CanSave = !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Price.ToString());
            }
        }
        private bool _cansave;
        public bool CanSave
        {
            get { return _cansave; }
            set { SetProperty(ref _cansave, value); }
        }
        public DelegateCommand SaveCommand { get; set; }
        INavigationService _navigationService;
        public TechAddCostPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            _navigationService = navigationService;
            SaveCommand = new DelegateCommand(Save).ObservesCanExecute(() => CanSave);
        }

        private async void Save()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding cost");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/itemcost/addcost/" + TechnicianModule.TenantName)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken).PostJsonAsync(new
                {
                    JobItemId = Issue.IssueId,
                    Cost = Price,
                    Description
                });
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    //Have to message the cost page
                    await _navigationService.GoBackAsync();

                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("jobitem"))
                {
                    Issue = ((Issue)parameters["jobitem"]);
                    Title = Issue.Title;
                }
            }
        }
    }
}
