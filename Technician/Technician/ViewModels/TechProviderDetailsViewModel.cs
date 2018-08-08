using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;

namespace TechTechnician.ViewModels
{
    public class TechProviderDetailsViewModel : BindableBase
    {
        public Property Provider { get; set; }
        public TechProviderDetailsViewModel()
        {
            Provider = TechnicianModule.Provider;
        }
    }
}
