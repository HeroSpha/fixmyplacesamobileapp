using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;
using Prism.Navigation;

namespace Client.ViewModels
{
    public class ClientDetailsViewModel : BindableBase
    {
        private Property _provider;
        public Property Provider
        {
            get { return _provider; }
            set { SetProperty(ref _provider, value); }
        }
        public ClientDetailsViewModel()
        {
            Provider = ClientModule.Provider;
        }

       
    }
}
