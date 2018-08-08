using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using Shared.Models;
using SharedCode.Models;

namespace Shared.Events
{
    public class AddCarEvent : PubSubEvent<Car>
    {
    }
}
