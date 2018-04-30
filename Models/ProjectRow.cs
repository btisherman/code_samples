using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVSM.API.Modules.AutoScheduler.Models
{
    public class ProjectRow
    {
        public string Order  { get; set; }
        public string QueueName { get; set; }
        public DateTime QueueStart { get; set; }
        public DateTime QueueFinish { get; set; }
        public Double Estimate { get; set; }
        public DateTime ScheduleStart { get; set; }
        public DateTime ScheduleComplete { get; set; }
        public Double  PrcComplete { get; set; }
        public string FabStatus { get; set; }
    }
}