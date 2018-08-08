using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedCode.Models;

namespace TechTechnician.ViewModels
{
    public class TechProfilePageViewModel : BindableBase
    {
        private Technicians technician;
        public Technicians Technician
        {
            get { return technician; }
            set { SetProperty(ref technician, value); }
        }
        public TechProfilePageViewModel()
        {
            Technician = TechnicianModule.Technician;
        }
    }
}
