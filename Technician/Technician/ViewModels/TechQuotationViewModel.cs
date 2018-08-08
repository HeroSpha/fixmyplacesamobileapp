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
using System.Collections.ObjectModel;
using System.Linq;
using TechTechnician;

namespace Technician.ViewModels
{
	public class TechQuotationViewModel : BindableBase
	{
        private ObservableCollection<Issue> issues;
        public ObservableCollection<Issue> Issues
        {
            get { return issues; }
            set { SetProperty(ref issues, value); }
        }
        private Issue issue;
        public Issue Issue
        {
            get { return issue; }
            set { SetProperty(ref issue, value);
                if(Issue != null)
                {
                    NavigationParameters para = new NavigationParameters
                    {
                        {"issue", Issue }
                    };
                    _navigationService.NavigateAsync("TechAddQuotation", para);
                    Issue = null;
                }
            }
        }
        INavigationService _navigationService;
       
        public TechQuotationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            GetQuotationIssues();
            
        }
        private async void GetQuotationIssues()
        {
            try
            {
                Acr.UserDialogs.UserDialogs.Instance.ShowLoading("Quotations");
                var issues = await ServerPath.Path
                    .AppendPathSegment("/api/issues/getquotationissues/" + TechnicianModule.TenantName)
                    .GetJsonListAsync();
                if(issues != null)
                {
                    var list = issues.Select(issue => new Issue
                    {
                        IssueId = issue.issueId,
                        Title = issue.title,
                        Description = issue.description,
                        Category = new Category { CategoryName = issue.category.categoryName }

                    });
                    Issues = new ObservableCollection<Issue>(list);
                }
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
            catch (Exception)
            {
                Acr.UserDialogs.UserDialogs.Instance.HideLoading();
            }
        }
      
    }
}
