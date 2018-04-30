using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TVSM.API.Modules.AutoScheduler.Models
{
    public class ScheduleOrderRow
    {
        public string OrderId { get; set; }
        public DateTime Created { get; set; }
        public Double Estimate { get; set; }
        public DateTime? SchStart { get; set; }
        public DateTime? SchComplete { get; set; }
        public  string Source { get; set; }
        public string GroupName { get; set; }
        public int DesignPercent { get; set; }
        public string FabStatus { get; set; }
    }
}