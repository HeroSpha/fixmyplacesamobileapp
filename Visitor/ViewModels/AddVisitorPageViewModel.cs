using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Models;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Visitor.ViewModels
{
    public class AddVisitorPageViewModel : BindableBase, INavigatingAware
	{
        private Customer tenant;
        public Customer Tenant
        {
            get { return tenant; }
            set
            {
                SetProperty(ref tenant, value);
            }
        }


        private string firstname;
        public string Firstname
        {
            get { return firstname; }
            set { SetProperty(ref firstname, value); }
        }
        private string lastname;
        public string Lastname
        {
            get { return lastname; }
            set { SetProperty(ref lastname, value); }
        }
        private Department department;
        public Department Department
        {
            get { return department; }
            set { SetProperty(ref department, value); }
        }
        private string idNumber;
        public string IdNumber
        {
            get { return idNumber; }
            set { SetProperty(ref idNumber, value); }
        }
        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { SetProperty(ref phoneNumber, value); }
        }
        private string regNumber;
        public string RegNumber
        {
            get { return regNumber; }
            set { SetProperty(ref regNumber, value); }
        }
        public DelegateCommand AddVisitorCommand { get; set; }
        private ObservableCollection<Department> departments;
        public ObservableCollection<Department> Departments
        {
            get { return departments; }
            set { SetProperty(ref departments, value); }
        }

        INavigationService _navigationService;
        public AddVisitorPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            AddVisitorCommand = new DelegateCommand(Add);
            GetDepartment();
        }
        private string gender;
        public string Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value); }
        }
        private async void GetDepartment()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading departments");
                var departments = await ServerPath.Path
                    .AppendPathSegment("/api/department/getdepartments/" + VisitorModule.TenantName)
                    .WithOAuthBearerToken(VisitorModule.AccessToken)
                    .GetJsonAsync<List<Department>>();
                
                Departments = new ObservableCollection<Department>(departments);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                throw new ApplicationException(ex.Message);
            }
        }
        private async void Add()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding visitor");
                var added = await ServerPath.Path
                    .AppendPathSegment("/api/visitors/addvisitor/" + VisitorModule.TenantName)
                    .PostJsonAsync(new
                    {
                        Firstname,
                        Lastname,
                        PhoneNumber,
                        IdNumber,
                        DateIn = DateTime.Now,
                        Tenant.CustomerId,
                        Department.DepartmentId,
                        Gender,
                        RegNumber
                    });
                if (added.IsSuccessStatusCode)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading(); ;
                    await _navigationService.NavigateAsync("VisitorsPage");
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("");
                }

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                Tenant = parameters["tenant"] as Customer;
            }
        }
    }
}
