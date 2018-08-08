using System;
using System.Collections.Generic;
using System.Text;

namespace Fixmyplacemobileapp.Models
{
    public class Security
    {
        public string SecurityId { get; set; }
        public int PropertyId { get; set; }
        public string Email { get; set; }
        public virtual Property Property { get; set; }
    }
}
