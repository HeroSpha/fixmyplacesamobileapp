using System;
using System.Collections.Generic;
using System.Text;
using SharedCode.Models;

namespace Shared.Models
{
    public class Quotation
    {
        public int QuotationId { get; set; }
        public string IssueId { get; set; }
        public string TechnicianId { get; set; }
        public double PriceOffered { get; set; }
        public string Description { get; set; }
        public virtual Technicians Technicians { get; set; }
    }
}
