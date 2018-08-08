using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class JobItem
    {
        public string JobItemId { get; set; }
        public string JobCardId { get; set; }
        public string IssueId { get; set; }
        public virtual JobCard JobCard { get; set; }
    }
}

