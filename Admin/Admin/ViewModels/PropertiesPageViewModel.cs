using Flurl;
using Flurl.Http;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Admin.ViewModels
{
    public class PropertiesPageViewModel : BindableBase
	{
        private Property property;
        public Property Property
        {
            get { return property; }
            set { SetProperty(ref property, value);
                if(Property != null)
                {
                    Admin.AdminModule.Logo = Property.Parent.Logo;
                    NavigationParameters nav = new NavigationParameters
                    {
                        
                        { "property", Property }
                    };
                    _navigationService.NavigateAsync("AdminDashboard", nav);
                    Property = null;
                }
            }
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { SetProperty(ref searchText, value);
                if (!string.IsNullOrEmpty(SearchText))
                {
                    var item = _placeholders.Where(p => p.TenantName.ToLower().Contains(SearchText.ToLower()) || p.LookupId.Contains(SearchText)).ToList();
                    Properties = new ObservableCollection<Property>(item);
                }
                else
                    Properties = _placeholders;
            }
        }
        public ObservableCollection<Property> _placeholders { get; set; }
        private ObservableCollection<Property> properties;
        public ObservableCollection<Property> Properties
        {
            get { return properties; }
            set { SetProperty(ref properties, value); }
        }
        INavigationService _navigationService;
        public PropertiesPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GetProperties();
        }
        private async void GetProperties()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var providers = await ServerPath.Path
                    .AppendPathSegment("/api/propertymanager/getmanagerproperties/" + AdminModule.UserId)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonAsync<List<Property>>();
                Properties = new ObservableCollection<Property>(providers);
                _placeholders = new ObservableCollection<Property>(Properties);
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync(ex.Message);
            }
        }
	}
}
