using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class DateStamp
    {
        public string DateStampId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsFullDay { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string JobItemId { get; set; }
        public virtual Item JobItem { get; set; }
    }
}
