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

namespace Fixmyplacemobileapp.ViewModels
{
    public class ChangePasswordPageViewModel : BindableBase
    {
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value);
                if (Password != null)
                {
                    CanChange = !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword);
                }
            }
        }
        private string conformPassword;
        public string ConfirmPassword
        {
            get { return conformPassword; }
            set
            {
                SetProperty(ref conformPassword, value);
                if (ConfirmPassword != null)
                {
                    CanChange = !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword);
                }
            }
        }

        private bool canChange;
        public bool CanChange
        {
            get { return canChange; }
            set
            {
                SetProperty(ref canChange, value);
                
            }
        }
        
        public DelegateCommand ChangePasswordCommand { get; set; }
        INavigationService _navigationService;
        public ChangePasswordPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            ChangePasswordCommand = new DelegateCommand(ChangePassword).ObservesCanExecute(() => CanChange);
        }

        private async void ChangePassword()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Changing password.");
                var result = await ServerPath.Path
                    .AppendPathSegment("/account/changepassword")
                    .WithOAuthBearerToken(App.Access_Token).PostJsonAsync(new
                {
                   UserName = Settings.Username,
                   Password,
                   ConfirmPassword
                }).ReceiveJson();
                var erro = result.errors;
                if(erro.Count > 0)
                {
                    StringBuilder _msg = new StringBuilder();
                    foreach (var _item in erro)
                    {
                        _msg.AppendLine(_item); 
                    }

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(_msg.ToString());
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    Settings.Password = string.Empty;
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Password changed");
                    await _navigationService.NavigateAsync("MainPage");
                }
                
                
               
            }
            catch (FlurlHttpException ex)
            {
                try
                {
                    var msg = await ex.Call.Response.Content.ReadAsStringAsync();
                 
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    StringBuilder _messages = new StringBuilder();
                    var errors = JObject.Parse(msg);

                   
                    
                    foreach(var key in errors["modelState"])
                    {

                      foreach(var val in key)
                        {
                            foreach(var item in val)
                            {
                                _messages.AppendLine(item.ToString());

                            }
                        }
                    }

                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(_messages.ToString());
                }
                catch (Exception exception)
                {

                  await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to change password, check if they match and try again.");
                }
            }
        }
    }
}
