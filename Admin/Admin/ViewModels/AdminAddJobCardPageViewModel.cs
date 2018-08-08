using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Navigation;
using SharedCode.Models;

namespace Admin.ViewModels
{
    public class AdminAddJobcardPageViewModel : BindableBase
    {

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value);
                CanSave = !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Name);
            }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value);
                CanSave = !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(Name);
            }
        }
        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetProperty(ref canSave, value); }
        }
        INavigationService _navigationService;
        public DelegateCommand NextCommand { get; set; }
        public AdminAddJobcardPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            NextCommand = new DelegateCommand(Next).ObservesCanExecute(() => CanSave);
        }

        private async void Next()
        {
            NavigationParameters para = new NavigationParameters();
            AdminModule.Jobcard = new JobCard { Name = Name, Description = Description };
            await _navigationService.NavigateAsync("AdminPickIssues");

        }
    }
}
