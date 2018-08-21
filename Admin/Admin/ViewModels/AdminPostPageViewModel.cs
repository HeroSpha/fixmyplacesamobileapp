using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using SharedCode.Models;
using Prism.Navigation;
using Prism.Events;
using SharedCode.Helpers;
using Flurl;

namespace Admin.ViewModels
{
    public class AdminPostPageViewModel : BindableBase, INavigationAware
    {
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { SetProperty(ref customer, value); }
        }
      
        private string _address = "";
        private List<Category> _categoryList;
        public List<Category> CategoryList
        {
            get { return _categoryList; }
            set { SetProperty(ref _categoryList, value); }
        }
        private Category _category;
        public Category Category
        {
            get { return _category; }
            set
            {
                SetProperty(ref _category, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                SetProperty(ref _title, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                CanSave = !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private bool _canSave;
        public bool CanSave
        {
            get { return _canSave; }
            set { SetProperty(ref _canSave, value); }
        }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public DelegateCommand SaveCommand { get; set; }
        public AdminPostPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
          
            _navigationService = navigationService;
            SaveCommand = new DelegateCommand(Save).ObservesCanExecute(() => CanSave);
            _eventAggregator = eventAggregator;
            GetCategories();
        }
        private async void GetCategories()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading categories");
                var categories = await ServerPath.Path
                    .AppendPathSegment("/api/categories/getcategories/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<Category>>();
                CategoryList = new List<Category>(categories);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();

            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No category found.");
            }

        }
     
        private async void Save()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Logging Issue");

                var post = await ServerPath.Path
                    .AppendPathSegment("/api/issues/addissuemobile/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .PostJsonAsync(new
                {
                    Title,
                    Description,
                    IsResolved = false,
                    Address = Customer.Unit,
                    Category.CategoryId,
                    JobPerformed = "",
                    PostedOn = DateTime.Now,
                    Status = "Ack",
                    Customer.CustomerId,
                    ImageUrl1 = "",
                    ImageUrl2 = "",
                    ImageUrl3 = "",
                    Longitude = "",
                    Latitude = ""
                });
                if (post != null)
                {
                    Title = string.Empty;
                    Description = string.Empty;
                    Category = null;
                    Address = null;
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Issue logged successfully.");
                }
                else
                {
                  await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Customer = parameters["customer"] as Customer;
            }
        }
    }
}
