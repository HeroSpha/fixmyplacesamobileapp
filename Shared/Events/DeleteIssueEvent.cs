using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using SharedCode.Models;

namespace SharedCode.Events
{
    public class DeleteIssueEvent : PubSubEvent<Issue>
    {
    }
}
