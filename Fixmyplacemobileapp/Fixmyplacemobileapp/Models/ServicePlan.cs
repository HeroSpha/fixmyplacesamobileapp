using System;
using System.Collections.Generic;
using System.Text;

namespace Fixmyplacemobileapp.Models
{
    public class ServicePlan
    {
        public ServicePlan()
        {
            Properties = new HashSet<Property>();
        }
        public int ServicePlanId { get; set; }
        public int TotalTenants { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Property> Properties { get; set; }
    }
}
