using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Events;
using Shared.Models;
using SharedCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Admin.ViewModels
{
	public class AdminVisitorDetailPageViewModel : BindableBase, INavigatingAware
	{
        private Visitor visitor;
        public Visitor Visitor
        {
            get { return visitor; }
            set { SetProperty(ref visitor, value); }
        }
        public DelegateCommand CheckoutCommnad { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;
        public AdminVisitorDetailPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            DeleteCommand = new DelegateCommand(Delete);
            CheckoutCommnad = new DelegateCommand(CheckOut).ObservesCanExecute(() => IsOut);
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
        }

        private async void Delete()
        {
            try
            {
               
                var confirm = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Delete visitor", "", "Delete", "Cancel", null);
                if(confirm)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Deleting");
                    var delete = await ServerPath.Path
                       .AppendPathSegment("/api/visitors/delete/" + AdminModule.TenantName + "/" + Visitor.Id)
                       .WithOAuthBearerToken(AdminModule.AccessToken)
                       .DeleteAsync();
                    if(delete.IsSuccessStatusCode)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Visitor deleted.");
                        await _navigationService.NavigateAsync("AdminVisitorPage");
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to deleted.");
                    }
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to delete");
            }
        }

        private bool isOut = false;
        public bool IsOut
        {
            get { return isOut; }
            set { SetProperty(ref isOut, value); }
        }
        private async void CheckOut()
        {
            try
            {
               
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Check out visitor?", "Checkout", "Checkout", "Cancel", null);
                if (result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Checking out");
                    Visitor.DateOut = DateTime.Now;
                    var checkedOut = await ServerPath.Path
                        .AppendPathSegment("/api/visitors/update/" + AdminModule.TenantName)
                        .WithOAuthBearerToken(AdminModule.AccessToken)
                        .PostJsonAsync(Visitor);
                    if (checkedOut.IsSuccessStatusCode)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        IsOut = false;
                        await _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminVisitorPage", useModalNavigation: true);
                    }
                    else
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Visitor checked out");
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminVisitorPage", useModalNavigation:true);
                    }
                       
                }
            }
            catch (Exception)
            {
                 Acr.UserDialogs.UserDialogs.Instance.HideLoading();
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to check out");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            try
            {
                if(parameters.GetNavigationMode() == NavigationMode.New)
                {
                    Visitor = parameters["visitor"] as Visitor;
                    if (visitor.DateOut == null)
                        IsOut = true;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
