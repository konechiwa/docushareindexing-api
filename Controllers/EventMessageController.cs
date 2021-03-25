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
    public class EventMessageController : BaseApiController
    {
        /**
        * @notice readonly variables.
        */
        private readonly IConfiguration _config;
        private readonly ITokenService _tokenService;
        public EventMessageController(IConfiguration config, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _config = config;
        }


        /**
        * @dev Return EventMessage object.
        * @param eventMessageDto The EventMessage data from request.
        */
        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<EventMessage>> addEventMessage(EventMessageDto eventMessageDto)
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Create SqlParameter object for output.
            SqlParameter paramEventID = new SqlParameter("@EventID", SqlDbType.Int);
            paramEventID.Direction = ParameterDirection.Output;

            SqlParameter paramEventTimestamp = new SqlParameter("@EventTimestamp", SqlDbType.DateTime);
            paramEventTimestamp.Direction = ParameterDirection.Output;


            // 3. Executing SqlCommand to database.
            await adapter.executedAsync("SP_EventMessage_Add", 
                CommandType.StoredProcedure, 
                new SqlParameter[] {
                    new SqlParameter("@EventType", eventMessageDto.EventType),
                    new SqlParameter("@EventKey", eventMessageDto.EventKey),
                    new SqlParameter("@EventStatus", eventMessageDto.EventStatus),
                    new SqlParameter("@EventDescription", eventMessageDto.EventDescription),
                    new SqlParameter("@UserID", eventMessageDto.UserID),
                    paramEventID,
                    paramEventTimestamp
                });
            
            // 4. Convert output value from SqlParameter to variable.
            int eventID = Convert.ToInt32(paramEventID.Value);
            DateTime eventTimestamp = Convert.ToDateTime(paramEventTimestamp.Value);


            // Final Return the EventMessage.
            return new EventMessage
            {
                EventID = eventID,
                EventTimestamp = eventTimestamp,
                EventType = eventMessageDto.EventType,
                EventKey = eventMessageDto.EventKey,
                UserID = eventMessageDto.UserID,
                EventStatus = eventMessageDto.EventStatus,
                EventDescription = eventMessageDto.EventDescription
            };
        }



        /**
        * @dev Return list of EventMessage objects.
        * @param eventKey That value is a key to search for events.
        */
        [Authorize]
        [HttpGet]
        [Route("find/{eventKey}")]
        public async Task<ActionResult<IEnumerable<EventMessage>>> findEventMessageByKey(string eventKey)
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Get stream data to DataSet object.
            DataSet dsSet = await adapter.getDataSetAsync(
                "SP_EventMessage_Find", 
                CommandType.StoredProcedure,
                new SqlParameter[] {
                    new SqlParameter("@EventKey", eventKey)
                });

            // 3. If not seek event key then return notfound object.
            if (dsSet == null && dsSet.Tables[0].Rows.Count == 0)
                return NotFound();

            // Final return events.
            return EventMessageHelper.convertDataSetToEventMessageList(dsSet);
        }


        /**
        * @dev Returns EventMessage History.
        */
        [Authorize]
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<EventMessage>>> getEventMessageHistory()
        {

            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Execute SqlCommand data to DataSet.
            DataSet dsSet = await adapter.getDataSetAsync(
                "SP_EventMessage_History", 
                CommandType.StoredProcedure);

            // Final Return the events
            return EventMessageHelper.convertDataSetToEventMessageList(dsSet);
        }

    }


    /**
    * @dev That class will be serialize stream data to objects.
    */
    public static class EventMessageHelper
    {
        public static List<EventMessage> convertDataSetToEventMessageList(DataSet dataset)
        {
            List<EventMessage> events = new List<EventMessage>();

            try
            {
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    events.Add(new EventMessage 
                    {
                        EventID = Convert.ToInt32(row["EventID"]),
                        EventTimestamp = Convert.ToDateTime(row["EventTimestamp"]),
                        EventType = row["EventType"].ToString(),
                        EventKey = row["EventKey"].ToString(),
                        UserID = row["UserID"].ToString(),
                        EventStatus = row["EventStatus"].ToString(),
                        EventDescription = row["EventDescription"].ToString()
                    });
                }

            } catch {}

            return events;
        }
    }
}