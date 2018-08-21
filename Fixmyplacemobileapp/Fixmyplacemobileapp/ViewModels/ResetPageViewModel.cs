using Fixmyplacemobileapp.Helpers;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fixmyplacemobileapp.ViewModels
{
    public class ResetPageViewModel : BindableBase
    {

        private bool canReset;
        public bool CanReset
        {
            get { return canReset; }
            set { SetProperty(ref canReset, value);
               
            }
        }
        private string email;
        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value);
               if(Email != null)
                {
                    CanReset = IsValid(Email);
                }
            }
        }
        public DelegateCommand ResetCommand { get; set; }
        INavigationService _navigationService;
        public ResetPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ResetCommand = new DelegateCommand(ResetPassword).ObservesCanExecute(() => CanReset);
        }

        private bool IsValid(string Email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(Email);
            if (match.Success)
                return true;
            else
                return false;
        }
        private async void ResetPassword()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Sending...");
                var result = await ServerPath.Path
                    .AppendPathSegment("/account/forgotpassword").PostJsonAsync(new
                {
                    Email = Email
                });
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Email reset successful.");
                    await _navigationService.NavigateAsync("myapp:///MainPage");
                }
                else if(result.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Email account not found,check email and try again.");
                }
            }
            catch (FlurlHttpException ex)
            {
                try
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    StringBuilder _messages = new StringBuilder();
                    var errors = JObject.Parse(ex.Call.Response.ToString());

                    var message = errors["error_description"];

                   await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(message.ToString());
                }
                catch (Exception exception)
                {

                   await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Email account not found,check email and try again.");
                }
            }
            
        }
    }
}
