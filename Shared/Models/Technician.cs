using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class Technicians
    {
        public Technicians()
        {
            JobCards = new HashSet<JobCard>();
        }
        public string TechnicianId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RegistrationId { get; set; }
        public virtual ICollection<JobCard> JobCards { get; set; }
    }
}
