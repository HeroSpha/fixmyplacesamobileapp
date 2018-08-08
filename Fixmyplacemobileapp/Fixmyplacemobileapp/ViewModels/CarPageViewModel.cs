using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Events;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fixmyplacemobileapp.ViewModels
{
	public class CarPageViewModel : BindableBase
	{
        private string make;
        public string Make
        {
            get { return make; }
            set { SetProperty(ref make, value);
                IsValid = !string.IsNullOrEmpty(Color) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Registration);
            }
        }
       
        private string color;
        public string Color
        {
            get { return color; }
            set { SetProperty(ref color, value);
                IsValid = !string.IsNullOrEmpty(Color) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Registration);
            }
        }
        private string registration;
        public string Registration
        {
            get { return registration; }
            set { SetProperty(ref registration, value);
                IsValid = !string.IsNullOrEmpty(Color) && !string.IsNullOrEmpty(Make) && !string.IsNullOrEmpty(Registration);
            }
        }
        private bool isValid;
        public bool IsValid
        {
            get { return isValid; }
            set { SetProperty(ref isValid, value);
            }
        }
        public DelegateCommand AddCarCommand { get; set; }
        IEventAggregator _eventAggregator;
        INavigationService _navigationService;
        public CarPageViewModel(IEventAggregator eventAggregator, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _eventAggregator = eventAggregator;
            AddCarCommand = new DelegateCommand(AddCar).ObservesCanExecute(() => IsValid);
        }
       
        private void AddCar()
        {
            var car = new Car
            {
                Model = "None",
                Make = Make,
                Color = Color,
                Registration = Registration
            };
            _eventAggregator.GetEvent<AddCarEvent>().Publish(car);
            _navigationService.GoBackAsync();
        }
    }
}
