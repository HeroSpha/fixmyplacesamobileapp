using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public class Setting
    {
        public int SettingId { get; set; }
        public bool ExternalTechAccount { get; set; }
        public bool ExternalTenantAccount { get; set; }
        public bool AutoPublish { get; set; }
        public int PropertyId { get; set; }
    }
}
