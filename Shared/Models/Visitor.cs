using System;
using System.Collections.Generic;
using System.Text;
using SharedCode.Models;

namespace Shared.Models
{
    public class Visitor
    {
        public long Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string IdNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
