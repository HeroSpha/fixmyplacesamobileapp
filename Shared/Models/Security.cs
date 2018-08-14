using System;
using System.Collections.Generic;
using System.Text;
using SharedCode.Models;

namespace Shared.Models
{
    public class Security
    {
        public string SecurityId { get; set; }
        public int PropertyId { get; set; }
        public string Email { get; set; }
        public virtual Property Property { get; set; }
    }
}
