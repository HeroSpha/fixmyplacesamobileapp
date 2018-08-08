using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Prism.Navigation;
using Flurl.Http;
using SharedCode.Helpers;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechEditPageViewModel : BindableBase, INavigatingAware
    {
        private IFlurlClient FlurlClient;
        private Technicians technician;
        public Technicians Technician
        {
            get { return technician; }
            set { SetProperty(ref technician, value); }
        }
        public DelegateCommand UpdateCommand { get; set; }
        public TechEditPageViewModel()
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            UpdateCommand = new DelegateCommand(UpdateAsync);
        }

        private async void UpdateAsync()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Updating");
                var result = await ServerPath.Path
                    .AppendPathSegment("/api/technicians/updatetechnician/" + TechnicianModule.Provider.TenantName).WithOAuthBearerToken(TechnicianModule.AccessToken).PostJsonAsync(new
                {
                    Name = Technician.Name,
                    Phone = Technician.Phone,
                    Email = Technician.Email,
                    Description = Technician.Description,
                    TechnicianId = Technician.TechnicianId,
                    RegistrationId = Technician.RegistrationId
                });
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Failed");
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
           if(parameters.GetNavigationMode() == NavigationMode.New)
            {
                Technician = parameters["tech"] as Technicians;

            }
        }
    }
}
