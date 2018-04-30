using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using TVSM.API.Modules.Application.Helpers;
using TVSM.API.Modules.AutoScheduler.Models;
using TVSM.Security;

namespace TVSM.API.Modules.AutoScheduler
{
    [RoutePrefix("api/autoscheduler")]
    public class AutoSchedulerController : ApiController    {
      
        [SkipXSRF]
        [Route("getschedule/{orderid}")]
        [HttpGet]
        public async Task<IHttpActionResult> GetScheduleForOrder(string orderid, CancellationToken cancellationToken)
        {

            string queryString = @"select Created, t.OrderID, TotEstmHrs,
calculatedEst = IIF(TotEstmHrs = 0, dbo.GetEstimateAPP(ToolCode, ToolNumber, Plansubtype), TotEstmHrs), Source, Group_Name as GroupName, tvsm.[ETVS %] as DesignPercent, f. [Plan Status] as FabStatus

 from (Select DATE_REVISED AS Created, OrderID, TotEstmHrs, ToolCode, ToolNumber, Plansubtype, Source = 'CAPP' from TED_PROD.MES_CAPP_ToolOrder
 
  UNION

 Select CreationDate AS Created, OrderID, TotEstmHrs, ToolCode, ToolNumber, Plansubtype, Source = 'SFM' from TED_PROD.MES_SFM_ToolOrder) t

left join
 (SELECT o.OrderID, o.GROUP_NAME
  FROM (
  select orderid, max(sequence_number) as maxsequence
  from TED_PROD.MES_CAPP_ToolOrder_InSignOff 
  where current_work is not null
  group by orderid
  ) as x inner join TED_PROD.MES_CAPP_ToolOrder_InSignOff as o on 
  o.OrderID = x.OrderID and o.sequence_number = x.maxsequence) o
  on t.[OrderID] = o.OrderID
 left join 
  tblTVSM tvsm
  on t.OrderID = tvsm.[Order ID]

left join tblTVSM f
  on tvsm.[Order ID] = f.[From Design] 
 

            where t.orderid = @orderid";

            using (IDbConnection connection = new DBConnection().OpenConnection())
            {
                var res = await connection.QueryAsync<ScheduleOrderRow>(
                    new CommandDefinition(string.Format(queryString), new { orderid = orderid }, cancellationToken: cancellationToken)
                   );

                var projectData = new List<ProjectRow>();
                var obj = res.FirstOrDefault();
                if (obj == null)
                {
                    return Ok(projectData);
                }
             
                var startDate = obj.Created;
                for (int i = 0; i <= 5; i++)
                {
                    if(i == 0)
                    {
                        var PrcComplete = obj.Source == "SFM" ? 100 : 0;
                        var GroupList = new List<string> { "R-ME", "R-ME-P8", "R-TE L FNL", "R-TE L WIN", "R-TE L MAX", "R-TE L P8A", "R-TE L AOG" };
                        if (!GroupList.Contains(obj.GroupName)){
                            PrcComplete = 100;
                        }
                        var queueData = new ProjectRow()
                        {
                            Order = obj.OrderId,
                            QueueName = "ME Planning",
                            QueueStart = startDate,
                            QueueFinish = startDate.AddDays(3),
                            Estimate = obj.Estimate,
                            PrcComplete = PrcComplete
                        };
                        startDate = queueData.QueueFinish;
                        projectData.Add(queueData);
                    }

                    if (i == 1)
                    {
                        var queueData = new ProjectRow()
                        {
                            Order = obj.OrderId,
                            QueueName = "Vetting",
                            QueueStart = startDate,
                            QueueFinish = startDate.AddDays(8),
                            Estimate = obj.Estimate,
                            PrcComplete = obj.Source == "SFM" ? 100 : 0
                        };
                        startDate = queueData.QueueFinish;
                        projectData.Add(queueData);
                    }

                    if (i == 2)
                    {
                        var PrcComplete = obj.DesignPercent | 0;
                        var flow = Math.Ceiling(obj.Estimate / 6.5);
                        var queueData = new ProjectRow()
                        {
                            Order = obj.OrderId,
                            QueueName = "Design",
                            QueueStart = obj.SchStart ?? startDate,
                            Estimate = obj.Estimate,
                            QueueFinish = obj.SchComplete ?? startDate.AddDays(flow + 10),
                            PrcComplete = PrcComplete 
                        };
                        startDate = queueData.QueueFinish;
                        projectData.Add(queueData);
                    }

                    if (i == 3)
                    {
                      
                        var queueData = new ProjectRow()
                        {
                            Order = obj.OrderId,
                            QueueName = "FAB Planning",
                            QueueStart = startDate,
                            QueueFinish = startDate.AddDays(3),
                            Estimate = obj.Estimate,
                            PrcComplete = obj.FabStatus == "UNV" ? 100 : 0

                        };
                        startDate = queueData.QueueFinish;
                        projectData.Add(queueData);
                    }

                    if (i == 4)
                    {
                        var PrcComplete = obj.DesignPercent | 0;
                        var est = obj.Estimate * (4.0 / 3.0);
                        var flow = Math.Ceiling(est / 6.5);
                        startDate = startDate.AddDays(10); //buffer
                        var queueData = new ProjectRow()
                        {
                            Order = obj.OrderId,
                            QueueName = "FAB",
                            QueueStart = startDate,
                            QueueFinish = startDate.AddDays(flow + 10),
                            Estimate = est,
                            PrcComplete = PrcComplete
                        };
                        startDate = queueData.QueueFinish;
                        projectData.Add(queueData);
                    }

                    if (i == 5)
                    {
                        var est = obj.Estimate * (2.0/3.0);
                        var flow = Math.Ceiling(est / 6.5);
                        startDate = startDate.AddDays(5); //buffer
                        var queueData = new ProjectRow()
                        {
                            Order = obj.OrderId,
                            QueueName = "Assembly",
                            QueueStart = startDate,
                            Estimate = est,
                            QueueFinish = startDate.AddDays(flow + 10)
                        };
                        startDate = queueData.QueueFinish;
                        projectData.Add(queueData);
                    }

                }

            

                //return another private function to supplement the current looping query while figuring out the first swim lane object.
                return Ok(projectData);
            }

        }
    }
}