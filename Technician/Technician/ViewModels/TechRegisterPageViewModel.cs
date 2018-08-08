using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Models;
using SharedCode.Helpers;
using Flurl.Http;
using Flurl;
using Shared.Models;

namespace TechTechnician.ViewModels
{
    public class TechRegisterPageViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                CanRegister = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Contact) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                CanRegister = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Contact) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }

        private string _contact;
        public string Contact
        {
            get { return _contact; }
            set
            {
                SetProperty(ref _contact, value);
                CanRegister = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Contact) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }
        private Technicians technicians;
        public Technicians Technicians
        {
            get { return technicians; }
            set { SetProperty(ref technicians, value); }
        }
        private string _email;
        public string Email
        {
            get { return _email; }
            set
            {
                SetProperty(ref _email, value);
                CanRegister = !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Contact) && !string.IsNullOrEmpty(Description) && Category != null;
            }
        }

        private bool _canregister;
        public bool CanRegister
        {
            get { return _canregister; }
            set { SetProperty(ref _canregister, value); }
        }
        private Category _category;
        public Category Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
        private List<Category> _categoryList;
        public List<Category> CategoryList
        {
            get { return _categoryList; }
            set { SetProperty(ref _categoryList, value); }
        }
        INavigationService _navigationService;
        public DelegateCommand RegisterCommand { get; set; }
        public TechRegisterPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            RegisterCommand = new DelegateCommand(Register);
            _navigationService = navigationService;
            GetCategories();
        }

        private async void RegisterCategories()
        {
            try
            {
                var list = new List<TechCategory>();
                foreach(var category in CategoryList.Where(p => p.IsSelected ==true))
                {
                    list.Add(new TechCategory { TechnicianId = TechnicianModule.UserId, CategoryId = category.CategoryId });
                }

              if(list.Count > 0)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating categories");
                    await ServerPath.Path
                        .AppendPathSegment("/api/techniciancategory/addrange/" + TechnicianModule.TenantName)
                        .WithOAuthBearerToken(TechnicianModule.AccessToken)
                        .PostJsonAsync(list);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                else
                {
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Select atleast one category");
                }

            }
            catch (Exception ex)
            {
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("");
            }
        }
        private async void Register()
        {

            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating profile");
                var _message = await ServerPath.Path
                    .AppendPathSegment("/api/technicians/addtechnician/" + TechnicianModule.TenantName).WithOAuthBearerToken(TechnicianModule.AccessToken).PostJsonAsync(new
                {
                    TechnicianId = TechnicianModule.UserId,
                    Name,
                    Description,
                    Email,
                    Phone = Contact
                });
                if (_message.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    RegisterCategories();
                    Technicians = new Technicians
                    {
                        TechnicianId = TechnicianModule.UserId,
                        Name = Name,
                        Description = Description,
                        Email = Email,
                        Phone = Contact,
                        RegistrationId = ""
                    };
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await _navigationService.NavigateAsync("TechDashboard");
                }
            }
            catch (Exception ex)
            {
              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error occured");
            }
        }

        private async void GetCategories()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var categories = await ServerPath.Path
                    .AppendPathSegment("/api/categories/getcategories/" + TechnicianModule.TenantName).WithOAuthBearerToken(TechnicianModule.AccessToken).GetJsonListAsync();
                if (categories != null)
                {
                    CategoryList = categories.Select(category => new Category
                    {
                        CategoryId = category.categoryId,
                        CategoryName = category.categoryName
                    }).ToList();
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No category found.");
            }

        }
    }
}
