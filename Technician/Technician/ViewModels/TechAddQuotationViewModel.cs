using Flurl;
using Flurl.Http;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Shared.Models;
using SharedCode.Helpers;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechTechnician;

namespace Technician.ViewModels
{
	public class TechAddQuotationViewModel : BindableBase, INavigatingAware
	{
       
        private bool canPost;
        public bool CanPost
        {
            get { return canPost; }
            set { SetProperty(ref canPost, value); }
        }
        private Issue issue;
        public Issue Issue
        {
            get { return issue; }
            set { SetProperty(ref issue, value);
            }
        }
        private double priceOffered;
        public double PriceOffered
        {
            get { return priceOffered; }
            set { SetProperty(ref priceOffered, value); }
        }
        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }
        private int   quotationId;
        public int QuotationId
        {
            get { return quotationId; }
            set { SetProperty(ref quotationId, value); }
        }
        public DelegateCommand PostCommand { get; set; }
        public TechAddQuotationViewModel()
        {
            PostCommand = new DelegateCommand(PostQuotation);
        }

        
        private async Task GetQuotation()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Get Quotation");
                var quotation = await ServerPath.Path
                .AppendPathSegment("/api/quotations/getquotation/" + TechnicianModule.TenantName + "/" + issue.IssueId + "/" + TechnicianModule.UserId)
                .GetJsonAsync<Quotation>();
                if (quotation != null)
                {
                    
                    PriceOffered = quotation.PriceOffered;
                    Description = quotation.Description;
                    QuotationId = quotation.QuotationId;
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
        private async void PostQuotation()
        {
            try
            {
                CanPost = (!string.IsNullOrEmpty(PriceOffered.ToString()) && !string.IsNullOrEmpty(Description));
               

                if (CanPost)
                {
                    Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Post quotation");
                   
                   
                    var quotatation = await ServerPath.Path
                        .AppendPathSegment("/api/quotations/add/" + TechnicianModule.TenantName)
                        .PostJsonAsync(new
                        {
                            QuotationId,
                            Issue.IssueId,
                            PriceOffered,
                            Description,
                            TechnicianId = TechnicianModule.UserId
                        });
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    if (quotatation.IsSuccessStatusCode)
                    {
                        await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Quotation submitted");
                    }
                   
                }
                else
                {
                    Acr.UserDialogs.UserDialogs.Instance.HideLoading();
                    await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("Values cannot be empty.");
                }
            }
            catch (Exception ex)
            {

               await Acr.UserDialogs.UserDialogs.Instance.AlertAsync("");
            }
          
        }

        public async void OnNavigatingTo(INavigationParameters parameters)
        {
            if (parameters.GetNavigationMode() == NavigationMode.New)
            {
                Issue = parameters["issue"] as Issue;
                await GetQuotation();
            }
        }
    }
}
