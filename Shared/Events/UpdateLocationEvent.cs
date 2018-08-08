using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;


namespace SharedCode.Events
{
    public class UpdateLocationEvent : PubSubEvent<string>
    {
    }
}
