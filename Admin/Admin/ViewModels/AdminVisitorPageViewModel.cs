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
	public class AdminVisitorPageViewModel : BindableBase
	{
        public ObservableCollection<Visitor> _visitors { get; set; }
        private ObservableCollection<Visitor> visitors;
        public ObservableCollection<Visitor> Visitors
        {
            get { return visitors; }
            set { SetProperty(ref visitors, value); }
        }
        private string searchKey;
        public string SearchKey
        {
            get { return searchKey; }
            set { SetProperty(ref searchKey, value);
                if (!string.IsNullOrEmpty(SearchKey))
                {
                    var _filtered = _visitors.Where(p => p.Firstname.ToLower().Contains(SearchKey.ToLower()) || p.Lastname.ToLower().Contains(SearchKey.ToLower()) || p.IdNumber.ToLower().Contains(SearchKey.ToLower()));
                    Visitors = new ObservableCollection<Visitor>(_filtered);
                }
                else
                {
                    Visitors = _visitors;
                }
            }
        }
        private Visitor visitor;
        public Visitor Visitor
        {
            get { return visitor; }
            set
            {
                SetProperty(ref visitor, value);
                if (Visitor != null)
                {
                    NavigationParameters para = new NavigationParameters
                    {
                        {"visitor", Visitor }
                    };
                    _navigationService.NavigateAsync("AdminMasterPage/NavigationPage/AdminVisitorDetailPage", para, useModalNavigation:true);
                    Visitor = null;
                }
            }
        }
        public DelegateCommand AddCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public AdminVisitorPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            GetVisitors();
            AddCommand = new DelegateCommand(Add);
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<UpdateVisitor>().Subscribe(Update);

        }

        private void Update(Visitor obj)
        {
            var item = Visitors.FirstOrDefault(p => p.Id == obj.Id);
            item.DateOut = obj.DateOut;
        }

        private void Add()
        {
            _navigationService.NavigateAsync("PickCustomerPage");
        }

        private async void GetVisitors()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading");
                var visitors = await ServerPath.Path
                    .AppendPathSegment("/api/visitors/getvisitors/" + AdminModule.TenantName)
                    .WithOAuthBearerToken(AdminModule.AccessToken)
                    .GetJsonListAsync();
                if(visitors != null)
                {
                    var list = visitors.Select(visitor => new Visitor
                    {
                        Id = visitor.id,
                        Firstname = visitor.firstname,
                        Lastname = visitor.lastname,
                        IdNumber = visitor.idNumber,
                        DateIn = visitor.dateIn,
                        DateOut = visitor.dateOut,
                        PhoneNumber = visitor.phoneNumber,
                        CustomerId = visitor.customerId,
                        Customer = new Customer { Firstname = visitor.customer.firstname, Lastname = visitor.customer.lastname }
                    });
                    Visitors = new ObservableCollection<Visitor>(list);
                    _visitors = new ObservableCollection<Visitor>(Visitors);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No visitors found");
            }
        }
	}
}
