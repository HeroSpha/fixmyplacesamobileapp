using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class Issue
    {
        public string IssueId { get; set; }
        public string Title { get; set; }
        public string CustomerId { get; set; }
        public string Description { get; set; }
        public bool IsResolved { get; set; }
        public string Address { get; set; }
        public string CategoryId { get; set; }
        public string JobPerformed { get; set; }
        public string Status { get; set; }
        public string ImageUrl1 { get; set; }
        public string ImageUrl2 { get; set; }
        public string ImageUrl3 { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime? DateResolved { get; set; }
        public virtual Category Category { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual JobItem JobItem { get; set; }
    }
}
