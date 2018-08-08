using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Models;
using SharedCode.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Client.ViewModels
{
	public class ClientVisitorPageViewModel : BindableBase
	{
        private ObservableCollection<Shared.Models.Visitor> visitors;
        public ObservableCollection<Shared.Models.Visitor> Visitors
        {
            get { return visitors; }
            set { SetProperty(ref visitors, value); }
        }
        private Visitor visitor;
        public Visitor Visitor
        {
            get { return visitor; }
            set { SetProperty(ref visitor, value);
               if(Visitor != null)
                {
                    Options();
                }
              
            }

        }
        private async void Options()
        {
            try
            {
                var result = await Acr.UserDialogs.UserDialogs.Instance.ActionSheetAsync("Options", "Cancel", "", null, "Check in", "Check out");
                switch(result)
                {
                    case "Check in":
                        {
                            await CheckOut("in");
                        }
                        break;
                    case "Check out":
                        {
                            await CheckOut("out");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again later.");
            }
        }
        private void Update(Visitor visitor)
        {
            var _update = Visitors.FirstOrDefault(p => p.Id == visitor.Id);
            var i = Visitors.IndexOf(_update);
            _update.DateOut = Visitor.DateOut;
            _update.DateIn = Visitor.DateIn;
            Visitors[i] = _update;
           
        }
        private async Task CheckOut(string Check)
        {
            try
            {
                switch(Check)
                {
                    case "in":
                        {
                            if (Visitor.DateIn != null && Visitor.DateOut == null)
                            {
                                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Visitor is already checked in.");
                                Visitor = null;
                                return;
                            }
                            else
                            {
                                Visitor.DateIn = DateTime.Now.AddHours(2);
                                Visitor.DateOut = null;
                            }
                        }
                        break;
                    case "out":
                        {
                            if(Visitor.DateIn == null || Visitor.DateIn != null && Visitor.DateOut != null)
                            {
                                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Visitor is already checked out.");
                                Visitor = null;
                                return;
                            }
                            else
                            {
                                Visitor.DateOut = DateTime.Now.AddHours(2);
                            }
                            
                        }
                        break;
                }
                var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync($"Check {Check} visitor?", $"Check {Check}", $"Check {Check}", "Cancel", null);
                if (result)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading($"Checking {Check}");
                   
                    var checkedOut = ServerPath.Path
                        .AppendPathSegment("/api/visitors/update/" + ClientModule.Tenant)
                        .WithOAuthBearerToken(ClientModule.AccessToken)
                        .PostJsonAsync(Visitor);
                    if (checkedOut != null)
                    {
                       
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        Update(Visitor);
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync($"Visitor checked {Check}");
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
                    }
                    Visitor = null;
                }
                Visitor = null;
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync($"Failed to check {Check}");
            }
        }
        public DelegateCommand AddVisitorCommand { get; set; }
        INavigationService _navigationService;
        public ClientVisitorPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            AddVisitorCommand = new DelegateCommand(AddVisitor);
            GetVisitors();
           
        }
        

        private async void AddVisitor()
        {
            await _navigationService.NavigateAsync("ClientAddVisitor");
        }
        private async void GetDepartment()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading visitors");
                var departments = await ServerPath.Path
                    .AppendPathSegment("/api/department/getdepartments/" + ClientModule.Tenant)
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonListAsync();
                if(departments != null)
                {
                    var list = departments.Select(department => new Department
                    {
                        Name = department.name,
                        DepartmentId = department.departmentId
                    });
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        private async void GetVisitors()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading visitors");
                var visitors = await ServerPath.Path
                    .AppendPathSegment("/api/visitors/getcustomervisitors/" + ClientModule.Tenant + "/" + ClientModule.UserId)
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonListAsync();
                if (visitors != null)
                {
                    var list = visitors.Select(visitor => new Shared.Models.Visitor
                    {
                        Id = visitor.id,
                        Firstname = visitor.firstname,
                        Lastname = visitor.lastname,
                        IdNumber = visitor.idNumber,
                        DateIn = visitor.dateIn,
                        DateOut = visitor.dateOut,
                        PhoneNumber = visitor.phoneNumber,
                        CustomerId = visitor.customerId
                    });
                    Visitors = new ObservableCollection<Shared.Models.Visitor>(list);
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No visitors found");
            }
        }
	}
}
