using System;
using System.Collections.Generic;
using System.Text;

namespace SharedCode.Models
{
    public class JobItemCost
    {
        public string CostId { get; set; }
        public string JobItemId { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
    }
}
