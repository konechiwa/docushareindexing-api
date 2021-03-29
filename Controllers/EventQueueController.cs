using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DocuShareIndexingAPI.Data;
using DocuShareIndexingAPI.DTOs;
using DocuShareIndexingAPI.Entities;
using DocuShareIndexingAPI.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DocuShareIndexingAPI.Controllers
{
    public class EventQueueController : BaseApiController
    {
        /**
        * @notice readonly variables.
        */        
        private readonly IConfiguration _config;

        public EventQueueController(IConfiguration config)
        {
            _config = config;
        }



        /**
        * @dev The function will return queue event collection.
        */
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventQueueDto>>> getEventQueue()
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Get stream data to DataSet object.
            DataSet dsSet = await adapter.getDataSetAsync(
                "SP_EventQueue_List", 
                CommandType.StoredProcedure);

            // Final return are queues.
            return EventQueueHelper.convertDataSetToEventQueue(dsSet);
        }


        [Authorize]
        [HttpPost]
        [Route("{id}")]
        public async Task<ActionResult<bool>> deleteEventQueue(int id)
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            await adapter.executedAsync("SP_EventQueue_Delete", CommandType.StoredProcedure,
            new SqlParameter[] { new SqlParameter("", id)});

            return Ok();
        }

    }



    /**
    * @notice This is class will be helping convert stream data to object. 
    */
    public static class EventQueueHelper
    {
        public static List<EventQueueDto> convertDataSetToEventQueue(DataSet dataset)
        {
            // 1. Create events objects.
            List<EventQueueDto> events = new List<EventQueueDto>();

            try
            {
                // 2. Convert datarow to event object.
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    events.Add(new EventQueueDto {
                        TrxNo = Convert.ToInt32(row["TrxNo"]),
                        EventID = Convert.ToInt32(row["EventID"]),
                        EventKey = row["EventKey"].ToString(),
                        EventCreateDate = Convert.ToDateTime(row["EventCreateDate"])
                    });
                }

            } catch {}

            // Final return the events.
            return events;
        }
    }
}