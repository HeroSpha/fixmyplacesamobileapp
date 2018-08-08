using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using XamForms.Controls;
using Prism.Navigation;
using SharedCode.Helpers;
using Xamarin.Forms;
using Flurl.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Flurl;

namespace TechTechnician.ViewModels
{
    public class TechCalendarPageViewModel : BindableBase
    {
        private IFlurlClient FlurlClient;

        private List<SpecialDate> _specialDates;
        public List<SpecialDate> SpecialDates
        {
            get { return _specialDates; }
            set { SetProperty(ref _specialDates, value); }
        }
        private IList<dynamic> dates;
        public IList<dynamic> Dates
        {
            get { return dates; }
            set { SetProperty(ref dates, value); }
        }
        private SpecialDate specialDate;
        public SpecialDate SpecialDate
        {
            get { return specialDate; }
            set
            {
                SetProperty(ref specialDate, value);
                if (SpecialDate != null)
                {
                    var dates = Dates.Select(date => new DateStamp
                    {
                        DateStampId = date.dateStampId,
                        IsFullDay = date.isFullDay,
                        Description = date.description,
                        StartDate = date.startDate,
                        EndDate = date.endDate,
                        JobItem = new Item { JobCardId = date.jobItem.jobCardId, IssueId = date.jobItem.issueId, JobItemId = date.jobItem.jobItemId },
                        JobItemId = date.jobItemid,
                        Title = date.title

                    }).ToList().Where(p => p.StartDate == SpecialDate.Date);
                }
                if (dates.Count > 0)
                {
                    NavigationParameters para = new NavigationParameters();
                    para.Add("dates", dates);
                    _navigationService.NavigateAsync("TechDatePage", para);
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.AlertAsync("No dates found");
                }

            }
        }
        public DelegateCommand<DateTime> DateCommand { get; set; }
        INavigationService _navigationService;
        public TechCalendarPageViewModel(INavigationService navigationService)
        {
            FlurlClient = new FlurlClient(ServerPath.Path);
            //DateCommand = new DelegateCommand<DateTime>(SelectedDate);
            _navigationService = navigationService;
            Getdates().ConfigureAwait(true);
           
        }

        private void SelectedDate(DateTime obj)
        {

        }

       
        private async Task Getdates()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Loading dates");
                var dates = await ServerPath.Path
                    .AppendPathSegment("/api/jobitems/techdates/" + TechnicianModule.TenantName + "/" + TechnicianModule.UserId)
                    .WithOAuthBearerToken(TechnicianModule.AccessToken)
                    .GetJsonListAsync();
                if (dates != null)
                {
                    Dates = dates;
                    var selectedDates = dates.Select(date => new SpecialDate(date.startDate)
                    {
                        BackgroundColor = Color.DarkRed,
                        Selectable = true,
                        TextColor = Color.White
                    });
                    SpecialDates = new List<SpecialDate>(selectedDates);
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                }


            }
            catch (Exception)
            {

              await  Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Error");
            }
        }
    }
}
