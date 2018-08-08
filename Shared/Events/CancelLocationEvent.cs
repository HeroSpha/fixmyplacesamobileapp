using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;

namespace SharedCode.Events
{
    public class CancelLocationEvent : PubSubEvent<bool>
    {
    }
}
