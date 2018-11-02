using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Models;
using SharedCode.Helpers;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminAddCostPageViewModel : BindableBase, INavigatingAware
    {
        
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
        public AdminAddCostPageViewModel(INavigationService navigationService)
        {
           
            _navigationService = navigationService;
            SaveCommand = new DelegateCommand(Save);
        }

        private async void Save()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding cost");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/itemcost/addcost/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken).PostJsonAsync(new
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
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("failed to add");
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                if (parameters.ContainsKey("issue"))
                {
                    Issue = ((Issue)parameters["issue"]);
                    Title = Issue.Title;
                }
            }
        }
    }
}
