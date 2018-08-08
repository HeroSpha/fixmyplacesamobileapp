using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using Shared.Models;

namespace Shared.Events
{
    public class UpdateVisitor : PubSubEvent<Visitor>
    {
    }
}
