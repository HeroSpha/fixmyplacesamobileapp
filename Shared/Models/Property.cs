using System;
using System.Collections.Generic;
using System.Text;
using Shared.Models;

namespace SharedCode.Models
{
    public class Property
    {
        public long PropertyId { get; set; }
        public string TenantName { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string LookupId { get; set; }
        public bool IsActive { get; set; }
        public string PoolName { get; set; }
        public bool? IsPublic { get; set; }
        public string Mac { get; set; }
        public string Android { get; set; }
        public string Windows { get; set; }
        public string Registrationid { get; set; }
        public virtual Parent Parent { get; set; }
        public virtual Setting Setting { get; set; }
        
    }
}
