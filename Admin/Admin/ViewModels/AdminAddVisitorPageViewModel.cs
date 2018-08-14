using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Events;
using Shared.Models;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Admin.ViewModels
{
	public class AdminAddVisitorPageViewModel : BindableBase, INavigatingAware
	{
        private Customer tenant;
        public Customer Tenant
        {
            get { return tenant; }
            set {
                SetProperty(ref tenant, value);
            }
        }
       

        private string firstname;
        public string Firstname
        {
            get { return firstname; }
            set { SetProperty(ref firstname, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private string lastname;
        public string Lastname
        {
            get { return lastname; }
            set { SetProperty(ref lastname, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private string idNumber;
        public string IdNumber
        {
            get { return idNumber; }
            set { SetProperty(ref idNumber, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private string phoneNumber;
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { SetProperty(ref phoneNumber, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private bool isVisible;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetProperty(ref isVisible, value); }
        }
        private bool addVisible;
        public bool AddVisible
        {
            get { return addVisible; }
            set { SetProperty(ref addVisible, value); }
        }
        public DelegateCommand RemoveCarCommand { get; set; }
        public DelegateCommand AddCarCommand { get; set; }
        public DelegateCommand AddVisitorCommand { get; set; }
        private ObservableCollection<Department> departments;
        public ObservableCollection<Department> Departments
        {
            get { return departments; }
            set { SetProperty(ref departments, value); }
        }
        private Department department;
        public Department Department
        {
            get { return department; }
            set { SetProperty(ref department, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private Car car;
        public Car Car
        {
            get { return car; }
            set { SetProperty(ref car, value);
                IsVisible = Car != null;
                AddVisible = !IsVisible;
            }
        }
        private string gender;
        public string Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
        }
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;
        public AdminAddVisitorPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            AddVisible = true;
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
            AddVisitorCommand = new DelegateCommand(Add).ObservesCanExecute(() => IsValid);
            RemoveCarCommand = new DelegateCommand(RemoveCar);
            AddCarCommand = new DelegateCommand(AddCar);
            GetDepartment();
            _eventAggregator.GetEvent<AddCarEvent>().Subscribe(AddCar);
        }

        private async  void RemoveCar()
        {
            var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove visitor car", "Remove", "Remove", "Cancel");
            if(result)
            {
                Car = null;
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Car removed");
            }
        }

        private async void AddCar()
        {
            await _navigationService.NavigateAsync("CarPage");
        }

      
        private void AddCar(Car obj)
        {
            Car = obj;
        }

        private async void GetDepartment()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading visitors");
                var departments = await ServerPath.Path
                    .AppendPathSegment("/api/department/getdepartments/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
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
                    .AppendPathSegment("/api/visitors/addvisitor/" + AdminModule.TenantName)
                    .PostJsonAsync(new
                    {
                        Firstname,
                        Lastname,
                        PhoneNumber,
                        IdNumber,
                        DateIn = DateTime.Now,
                        Tenant.CustomerId,
                        Department.DepartmentId,
                        Gender
                    }).ReceiveJson<Visitor>();
                Firstname = string.Empty;
                lastname = string.Empty;
                PhoneNumber = string.Empty;
                IdNumber = string.Empty;
                Car = null;
                Department = null;
                Gender = string.Empty;
                if (added != null && Car != null)
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    
                    Car.CarId = (int)added.Id;
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding car");
                    var _car = await ServerPath.Path
                        .AppendPathSegment("/api/car/add/" + AdminModule.TenantName).PostJsonAsync(Car).ReceiveJson();
                    if (_car != null)
                    {
                        await _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminVisitorPage", useModalNavigation:true);
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to add car");
                    }
                   
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Visitor added.");
                    await _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminVisitorPage", useModalNavigation: true);

                }
               
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Tenant = parameters["tenant"] as Customer;
            }
        }
    }
}
