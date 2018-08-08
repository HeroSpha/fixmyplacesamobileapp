using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class Notification
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string Priority { get; set; }
        public DateTime PostedOn { get; set; }
    }
}
