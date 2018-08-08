using System.Collections.Generic;
using Admin.Helpers;
using Admin.ViewModels;
using Admin.Views;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Prism.Ioc;
using Prism.Modularity;
using SharedCode.Models;

namespace Admin
{

    public class AdminModule : IModule
    {
       
        public static Property Provider { get; set; }
        public static HubProxy hubProxy { get; set; }
        public static string AccessToken { get; set; }
        public static string UserId { get; set; }
        public static string TenantName { get; set; }
        public static List<Issue> Issues { get; set; }
        public static JobCard Jobcard { get; set; }
        public static string Role { get; internal set; }
      

        //Display maps
        public static double? Latitude { get; set; }
        public static double? Longitude { get; set; }

       
        public void Initialize()
        {
           
        }

        public static string Logo { get; internal set; }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<AdminErrorPage>();
            containerRegistry.RegisterForNavigation<AdminDashboard>();
            containerRegistry.RegisterForNavigation<AdminAddCostPage>();
            containerRegistry.RegisterForNavigation<AdminAddNotification>();
            containerRegistry.RegisterForNavigation<AdminCalendar>();
            containerRegistry.RegisterForNavigation<AdminIssuesPage>();
            containerRegistry.RegisterForNavigation<AdminJobcard>();
            containerRegistry.RegisterForNavigation<AdminJobcardItems>();
            containerRegistry.RegisterForNavigation<AdminNotificationPage>();
            containerRegistry.RegisterForNavigation<AdminSupportPage>();
            containerRegistry.RegisterForNavigation<AdminCostPage>();
            containerRegistry.RegisterForNavigation<AdminNotoficationDetail>();
            containerRegistry.RegisterForNavigation<AdminAddJobcardPage>();
            containerRegistry.RegisterForNavigation<AdminPickIssues>();
            containerRegistry.RegisterForNavigation<AdminAddTechnician>();
            containerRegistry.RegisterForNavigation<AdminSettingsPage>();
            containerRegistry.RegisterForNavigation<AdminEditPage>();
            containerRegistry.RegisterForNavigation<AdminRegisterPage>();
            containerRegistry.RegisterForNavigation<AdminPostPage>();
            containerRegistry.RegisterForNavigation<PropertiesPage, PropertiesPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminVisitorPage, AdminVisitorPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminVisitorDetailPage, AdminVisitorDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminAddVisitorPage, AdminAddVisitorPageViewModel>();
            containerRegistry.RegisterForNavigation<PickCustomerPage, PickCustomerPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminTenantsPage, AdminTenantsPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminIssueDetailPage, AdminIssueDetailPageViewModel>();
            containerRegistry.RegisterForNavigation<AddJobcardPage, AddJobcardPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminMasterPage, AdminMasterPageViewModel>();
            containerRegistry.RegisterForNavigation<AdminPicTechnicinPage, AdminPicTechnicinPageViewModel>();

        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            AccessToken = Settings.AccessToken;
            UserId = Settings.UserId;
            Role = Settings.Role;
           
        }
    }
}
