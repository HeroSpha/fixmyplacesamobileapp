using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Fixmyplacemobileapp.Models;
using Prism.Navigation;
using System.Text;
using Newtonsoft.Json.Linq;
using Fixmyplacemobileapp.Helpers;
using Flurl.Http;

namespace Fixmyplacemobileapp.ViewModels
{
    public class SignUpPageViewModel : BindableBase
    {
        public ObservableCollection<RoleModel> Roles { get; set; }
        private RoleModel selectedRole;

        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value);
                CanRegister = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && SelectedRole != null;
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value);
                CanRegister = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && SelectedRole != null;
            }
        }
        private string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set { SetProperty(ref confirmPassword, value);
                CanRegister = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && SelectedRole != null;
            }
        }
        public RoleModel SelectedRole
        {
            get { return selectedRole; }
            set { SetProperty(ref selectedRole, value);
                CanRegister = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(ConfirmPassword) && SelectedRole != null;
            }
        }
        public DelegateCommand RegisterCommand { get; set; }
        INavigationService _navigationService;
        public SignUpPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            RegisterCommand = new DelegateCommand(Register).ObservesCanExecute(() => CanRegister);
            Roles = new ObservableCollection<RoleModel>
            {
                 new RoleModel{ RoleId =1, RoleName = "Client"},
                new RoleModel{ RoleId =2, RoleName = "Technician"},

            };
        }
        private bool _canRegister = false;
        public bool CanRegister
        {
            get { return _canRegister; }
            set { SetProperty(ref _canRegister, value); }
        }
        private async void Register()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Registering");
                string _url = ServerPath.Path + "/account/register/" + SelectedRole.RoleName;
                var message = await _url.PostJsonAsync(new
                {
                    UserName = Username,
                    Password = Password,
                    ConfirmPassword = ConfirmPassword
                }).ReceiveString();
                if (message != null)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await _navigationService.NavigateAsync("MainPage");
                }


            }
            catch (FlurlHttpException ex)
            {
                StringBuilder _messages = new StringBuilder();
                var errors = JObject.Parse(ex.Call.Response.ToString());
                var messages = errors["modelState"].Values<JToken>();
                var jarray = messages.Values<JToken>();
                foreach (JToken content in jarray.Children<JToken>())
                {
                    _messages.AppendLine(content.ToString());
                }

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync(_messages.ToString());
            }
        }
    }
}
