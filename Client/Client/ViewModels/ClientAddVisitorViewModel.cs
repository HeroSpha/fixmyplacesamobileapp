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
using System.Collections.ObjectModel;
using System.Linq;

namespace Client.ViewModels
{
	public class ClientAddVisitorViewModel : BindableBase
	{
        private ObservableCollection<Department> departments;
        public ObservableCollection<Department> Departments
        {
            get { return departments; }
            set { SetProperty(ref departments, value); }
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
       
        private Car car;
        public Car Car
        {
            get { return car; }
            set { SetProperty(ref car, value);
               if(Car != null)
                {
                    IsVisible = Car != null;
                    
                    CanAdd = !IsVisible;
                }
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
        private string gender;
        public string Gender
        {
            get { return gender; }
            set { SetProperty(ref gender, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private bool isVisble;
        public bool IsVisible
        {
            get { return isVisble; }
            set { SetProperty(ref isVisble, value);
                IsValid = !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
            }
        }
        private bool canAdd;
        public bool CanAdd
        {
            get { return canAdd; }
            set { SetProperty(ref canAdd, value); }
        }
        public DelegateCommand RemoveCarCommand { get; set; }
        public DateTime? DateOut { get; set; }
        public DateTime? DateIn { get; set; }
        public DelegateCommand AddCarCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public ClientAddVisitorViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            CanAdd = true;
           
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(SaveVisitorAsync).ObservesCanExecute(() => IsValid);
            AddCarCommand = new DelegateCommand(AddCar);
            RemoveCarCommand = new DelegateCommand(RemoveCar);
            GetDepartment();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AddCarEvent>().Subscribe(CarAdded);
        }

        private async void RemoveCar()
        {
            var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove visitor car", "Remove", "Remove", "Cancel");
            if (result)
            {
                Car = null;
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Car removed");
            }
        }

        private async void AddCar()
        {
            await _navigationService.NavigateAsync("CarPage");

        }

        private void CarAdded(Car obj)
        {
            Car = obj;
        }

        private Department department;
        public Department Department
        {
            get { return department; }
            set { SetProperty(ref department, value); }
        }
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value); }
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
                if (departments != null)
                {
                    var list = departments.Select(department => new Department
                    {
                        Name = department.name,
                        DepartmentId = department.departmentId
                    });
                    Departments = new ObservableCollection<Department>(list);
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                throw new ApplicationException(ex.Message);
            }
        }
        private bool Valid()
        {
            return !string.IsNullOrEmpty(Firstname) && !string.IsNullOrEmpty(Lastname) && !string.IsNullOrEmpty(Gender) && !string.IsNullOrEmpty(PhoneNumber) && Department != null && !string.IsNullOrEmpty(IdNumber);
        }
        private async void SaveVisitorAsync()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Add visitor");
                var addVisitor = await ServerPath.Path
                         .AppendPathSegment("/api/visitors/addvisitor/" + ClientModule.Tenant)
                        .WithOAuthBearerToken(ClientModule.AccessToken)
                        .PostJsonAsync(new
                        {
                            Firstname,
                            Lastname,
                            IdNumber,
                            PhoneNumber,
                            DateOut,
                            DateIn,
                            CustomerId = ClientModule.UserId,
                            Department.DepartmentId,
                            Gender
                        }).ReceiveJson<Visitor>();
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
               
                if (addVisitor != null && Car != null)
                {
                    Car.CarId = (int)addVisitor.Id;
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Adding car");
                    var _car = await ServerPath.Path
                        .AppendPathSegment("/api/car/add/" + ClientModule.Tenant).PostJsonAsync(Car).ReceiveJson();
                    if (_car != null)
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await _navigationService.NavigateAsync("ClientVisitorPage");
                    }
                    else
                    {
                        Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to add car");
                    }
                    
                }
                else
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Visitor added.");
                    await _navigationService.NavigateAsync("ClientVisitorPage");
                }

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed to add visitor");
            }
        }
    }
}
