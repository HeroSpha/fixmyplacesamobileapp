using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SharedCode.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fixmyplacemobileapp.ViewModels
{
	public class ImagesPageViewModel : BindableBase, INavigatingAware
	{
        private string image1;
        public string Image1
        {
            get { return image1; }
            set { SetProperty(ref image1, value); }
        }
        private string image2;
        public string Image2
        {
            get { return image2; }
            set { SetProperty(ref image2, value); }
        }
        private string image3;
        public string Image3
        {
            get { return image3; }
            set { SetProperty(ref image3, value); }
        }
        public ImagesPageViewModel()
        {

        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            var issue = parameters["issue"] as Issue;
            Image1 = issue.ImageUrl1;
            Image2 = issue.ImageUrl2;
            Image3 = issue.ImageUrl3;
        }
    }
}
