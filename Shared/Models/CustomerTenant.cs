using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class CustomerTenant
    {
        public int Id { get; set; }
       
        public string CustomerId { get; set; }
       
        public string TenantId { get; set; }
       
        public bool IsConfirmed { get; set; }
        public string Role { get; set; }
    }
}
