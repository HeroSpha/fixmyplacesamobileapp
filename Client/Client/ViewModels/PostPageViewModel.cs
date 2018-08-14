using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Flurl.Http;
using SharedCode.Models;
using Prism.Navigation;
using Prism.Events;
using System.Threading.Tasks;
using SharedCode.Helpers;
using Flurl;

namespace Client.ViewModels
{
    public class PostPageViewModel : BindableBase
    {
        
        private string _address = "Unknown";
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
        public PostPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
           
            _navigationService = navigationService;
            SaveCommand = new DelegateCommand(Save);
            _eventAggregator = eventAggregator;
            GetCategories().ConfigureAwait(true);
        }
        private async Task GetCategories()
        {
            try
            {

                var categories = await ServerPath.Path
                    .AppendPathSegment("/api/categories/getcategories/FixmyPlace Support")
                    .WithOAuthBearerToken(ClientModule.AccessToken)
                    .GetJsonAsync<List<Category>>();
                CategoryList = new List<Category>(categories);

            }
            catch (Exception ex)
            {
               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No category found.");
            }

        }
        private async void Save()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Logging Issue");

                var post = await ServerPath.Path
                    .AppendPathSegment("/api/issues/addissuemobile/FixmyPlace Support")
                    .WithOAuthBearerToken(ClientModule.AccessToken).PostJsonAsync(new
                {
                        Title,
                        Description,
                    IsResolved = false,
                        Address,
                        Category.CategoryId,
                    JobPerformed = "",
                    PostedOn = DateTime.Now,
                    Status = "Ack",
                    CustomerId = ClientModule.UserId,
                    ImageUrl1 = "",
                    ImageUrl2 = "",
                    ImageUrl3 = "",
                    Longitude = "",
                    Latitude = ""
                });
                if (post != null)
                {

                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
                else
                {
                  await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("An error occured, please try again.");
                }
            }
            catch (Exception ex)
            {

                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }
    }
}
