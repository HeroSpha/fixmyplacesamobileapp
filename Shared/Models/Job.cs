using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class JobCard
    {
        public string JobCardId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string TechnicianId { get; set; }
        public DateTime Date { get; set; }
      
    }
}
