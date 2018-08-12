using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using Newtonsoft.Json.Linq;
using Fixmyplacemobileapp.Helpers;
using Flurl.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Prism.Modularity;
using Flurl;
using Microsoft.AppCenter.Analytics;

namespace Fixmyplacemobileapp.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private string _title = "Login";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value);
                CanLogin = !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Username);
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value);
                CanLogin = !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Username);
            }
        }

        private bool canLogibn;
        public bool CanLogin
        {
            get { return canLogibn; }
            set { SetProperty(ref canLogibn, value); }
        }
        public DelegateCommand SignInCommand { get; set; }
        public DelegateCommand ResetCommand { get; set; }
        public DelegateCommand SignUpCommand { get; set; }
        INavigationService _navigationService;
        IModuleManager _moduleManager;
        public string Role { get; set; } = "";
        public MainPageViewModel(INavigationService navigationService, IModuleManager moduleManager)
        {
            if (Settings.Username != null && Settings.Password != null)
            {
                Username = Settings.Username;
                Password = Settings.Password;

            }
            _moduleManager = moduleManager;
            _navigationService = navigationService;
            SignInCommand = new DelegateCommand(SignIn);
            SignUpCommand = new DelegateCommand(SignUp);

            ResetCommand = new DelegateCommand(ResetPassword);
           
                                               //login
        SignInCommand = new DelegateCommand(Login).ObservesCanExecute(() => CanLogin);
            SignUpCommand = new DelegateCommand(() =>
            {
                _navigationService.NavigateAsync("NavigationPage/SignUpPage");
            });
        }

        private async void ResetPassword()
        {
            await _navigationService.NavigateAsync("NavigationPage/ResetPage");
        }

        private async void Login()
        {
            try
            {

                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Logging");
                var _message = await ServerPath.Path
                    .AppendPathSegment("/token")
                    .PostUrlEncodedAsync(new
                {
                    UserName = Username,
                    Password,
                    grant_type = "password"
                }).ReceiveString();

                if (_message != null)
                {

                    var json = JObject.Parse(_message);

                    App.Access_Token = json["access_token"].ToString();
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Getting user...");
                    var user = await ServerPath.Path
                        .AppendPathSegment("/account/username")
                        .WithOAuthBearerToken(App.Access_Token).GetJsonAsync();

                    if (user != null)
                    {
                        App.UserId = user.id;
                        Settings.UserId = user.id;
                        Settings.AccessToken = App.Access_Token;
                        Settings.Username = Username;
                        Settings.Password = Password;
                        var roles = user.roles[0];
                        if (roles != null)
                        {
                            string id = roles.roleId;
                            Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Getting user role");
                            //var update = await App.FlurlClient.WithUrl(ServerPath.Path + "/api/tenant/updateid").WithOAuthBearerToken(App.Access_Token).GetStringAsync();
                            var _role = await ServerPath.Path
                                .AppendPathSegment("/account/getrole/" + id)
                                .WithOAuthBearerToken(App.Access_Token).GetStringAsync();
                            if (_role != "")
                            {
                                Role = _role.Replace(@"""", "").Trim();
                            }
                        }
                        else
                        {
                           await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Role not found");
                        }
                    }
                    else
                    {
                       await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("User not found");
                    }
                    //wiil map signal R in a module
                    //await MapSignalR(App.UserId);

                    if (Role != "")
                    {

                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        switch (Role)
                        {
                            case "Caretaker":
                                {
                                    LoadModule("AdminModule");
                                    await _navigationService.NavigateAsync("NavigationPage/PropertiesPage");
                                }
                                break;
                            case "Security":
                                {
                                    LoadModule("VisitorModule");
                                    await _navigationService.NavigateAsync("NavigationPage/VisitorsPage");
                                }
                                break;
                            case "Technician":
                                {
                                    LoadModule("TechnicianModule");

                                    await _navigationService.NavigateAsync("NavigationPage/TechProviders");
                                }
                                break;
                            case "Client":
                                {
                                    LoadModule("ClientModule");
                                    try
                                    {
                                        await _navigationService.NavigateAsync("NavigationPage/ClientProvidersPage");
                                    }
                                    catch (Exception ex)
                                    {

                                        throw;
                                    }

                                }
                                break;
                        }
                        Settings.Role = Role;
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No role assigned");
                    }


                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Username or password incorrect.");
                }
                  
            }
            catch (FlurlHttpException ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Username or password incorrect.");
            }
        }

        private void LoadModule(string Module)
        {
            try
            {
                _moduleManager.LoadModule(Module);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private async Task MapSignalR(string UserId)
        {
            try
            {
                var hubConnection = new HubConnection(ServerPath.Path + "/signalr");
                App.hubProxy = hubConnection.CreateHubProxy("MyHub") as HubProxy;
                App.hubProxy.On<string>("addJobCard", text =>
                {
                    Acr.UserDialogs.UserDialogs.Instance.Toast("New jobcard created.");
                });
                await hubConnection.Start();
                await App.hubProxy.Invoke("subscribe", UserId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async void SignUp()
        {
            await _navigationService.NavigateAsync("SignUpPage02");
        }

        private async void SignIn()
        {
            var _results = await Acr.UserDialogs.UserDialogs.Instance.LoginAsync("Login to FixmyPlace", null, null);
           
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {

        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"] + " and Prism";
        }
    }
}
