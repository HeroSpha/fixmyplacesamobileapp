using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Events;
using SharedCode.Events;

namespace Client.ViewModels
{
    public class ClientLocationPageViewModel : BindableBase
    {
        public bool CanSave { get; set; }
        public DelegateCommand<object> MapClickedCommand { get; set; }
        public DelegateCommand CancelLocationCommand { get; set; }
        INavigationService _navigationService;
        IEventAggregator _eventAggregator;
        public ClientLocationPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationService = navigationService;
          //  MapClickedCommand = new DelegateCommand<object>(GetLocation);
            CancelLocationCommand = new DelegateCommand(CancelLocation);
            
        }

        private async void CancelLocation()
        {
            var result = await Acr.UserDialogs.UserDialogs.Instance.ConfirmAsync("Remove selected location", "Cancel", "Remove", "Cancel", null);
            if(result)
            {
              
                _eventAggregator.GetEvent<CancelLocationEvent>().Publish(true);
            }
        }

        //public Position Position { get; set; }
       
        //private void GetLocation(object Point)
        //{
        //    try
        //    {
        //        Position = ((Position)Point);
        //        _eventAggregator.GetEvent<UpdateLocationEvent>().Publish(Position);

        //    }
        //    catch (Exception)
        //    {
        //    }
           
        //}
    }
}
