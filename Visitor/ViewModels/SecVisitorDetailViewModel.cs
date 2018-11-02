using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Events;
using SharedCode.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Visitor.ViewModels
{
	public class SecVisitorDetailViewModel : BindableBase,INavigatingAware
	{
        private Shared.Models.Visitor visitor;
        public Shared.Models.Visitor Visitor
        {
            get { return visitor; }
            set { SetProperty(ref visitor, value); }
        }
        public DelegateCommand CheckoutCommnad { get; set; }
        IEventAggregator _eventAggregator;
        public SecVisitorDetailViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            CheckoutCommnad = new DelegateCommand(CheckOut).ObservesCanExecute(() => IsOut);
            _eventAggregator = eventAggregator;
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
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Checking out");
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Check out visitor?", "Checkout", "Checkout", "Cancel", null);
                if (result)
                {
                    Visitor.DateOut = DateTime.Now;
                    var checkedOut = ServerPath.Path
                        .AppendPathSegment("/api/visitors/update/" + VisitorModule.TenantName)
                        .WithOAuthBearerToken(VisitorModule.AccessToken)
                        .PostJsonAsync(Visitor);
                    if (checkedOut != null)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading(); ;
                        IsOut = false;
                        _eventAggregator.GetEvent<UpdateVisitor>().Publish(Visitor);
                    }
                    else
                       await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
                }
            }
            catch (Exception)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to check out");
            }
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            try
            {
                if (parameters.GetNavigationMode() == NavigationMode.New)
                {
                    Visitor = parameters["visitor"] as Shared.Models.Visitor;
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
