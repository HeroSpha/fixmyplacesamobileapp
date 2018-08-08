using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Events;
using SharedCode.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Visitor.ViewModels
{
	public class VisitorsPageViewModel : BindableBase
	{
        private ObservableCollection<Shared.Models.Visitor> visitors;
        public ObservableCollection<Shared.Models.Visitor> Visitors
        {
            get { return visitors; }
            set { SetProperty(ref visitors, value); }
        }
        private Shared.Models.Visitor visitor;
        public Shared.Models.Visitor Visitor
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
                    _navigationService.NavigateAsync("SecVisitorDetail", para);
                    Visitor = null;
                }
            }
        }
        public DelegateCommand AddCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public VisitorsPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            GetProperty().ContinueWith(async (met) =>
            {
                await GetVisitors();
            });
            AddCommand = new DelegateCommand(Add);
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<UpdateVisitor>().Subscribe(Update);

        }

        private void Update(Shared.Models.Visitor obj)
        {
            var item = Visitors.FirstOrDefault(p => p.Id == obj.Id);
            item.DateOut = obj.DateOut;
        }

        private void Add()
        {
            _navigationService.NavigateAsync("VisitorTenantPage");
        }
        private async Task GetProperty()
        {
            try
            {

                var property = await ServerPath.Path
                    .AppendPathSegment("/api/security/get/" + VisitorModule.UserId)
                    .WithOAuthBearerToken(VisitorModule.AccessToken)
                    .GetJsonAsync();
                if (property != null)
                {
                    VisitorModule.TenantName = property.property.tenantName;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        private async Task GetVisitors()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading visitors");
                var visitors = await ServerPath.Path
                    .AppendPathSegment("/api/visitors/getvisitors/" + VisitorModule.TenantName)
                    .WithOAuthBearerToken(VisitorModule.AccessToken)
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
                        CustomerId = visitor.customerId,
                        Customer = new SharedCode.Models.Customer { Firstname = visitor.customer.firstname, Lastname = visitor.customer.lastname }
                    });
                    Visitors = new ObservableCollection<Shared.Models.Visitor>(list);
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

                await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No visitors found");
            }
        }
    }
}
