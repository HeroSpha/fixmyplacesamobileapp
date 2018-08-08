using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fixmyplacemobileapp.Models
{
    public class Property
    {
        public string PropertyId { get; set; }
        public string TenantName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string LookupId { get; set; }
        public bool? IsPublic { get; set; }
        public int CategoryId { get; set; }
        public int ServicePlanId { get; set; }
        public string RegistrationId { get; set; }
        public string ParentId { get; set; }

        public virtual Setting Setting { get; set; }
        public virtual ServicePlan ServicePlan { get; set; }

    }
}
