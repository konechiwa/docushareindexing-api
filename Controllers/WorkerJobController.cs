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
    public class WorkerJobController : BaseApiController
    {
        private readonly IConfiguration _config;
        public WorkerJobController(IConfiguration config)
        {
            _config = config;
        }


        /**
        * @dev Returns worker job information.
        */
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkerJobDto>>> getWorkerJobs()
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Read streamdata from database to DataSet.
            DataSet dsSet = await adapter.getDataSetAsync("SP_WorkerJob_Select", CommandType.StoredProcedure);

            // Final result is workers.
            return WorkerJobHelper.convertDataSetToWorkerJobs(dsSet);
        }



        /**
        * @dev Returns worker after processing is complete.
        */
        [Authorize]
        [HttpPost("add")]
        public async Task<ActionResult<WorkerJobDto>> addWorkerJob(WorkerJobDto worker)
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Create SqlParameter output object.
            SqlParameter paramTrxNo = new SqlParameter("@TrxNo", SqlDbType.Int);
            paramTrxNo.Direction = ParameterDirection.Output;

            SqlParameter paramCreateDateTime = new SqlParameter("@CreateDateTime", SqlDbType.DateTime);
            paramCreateDateTime.Direction = ParameterDirection.Output;


            // 3. Execute new worker job to database.
            await adapter.executedAsync("SP_WorkerJob_Add", CommandType.StoredProcedure,
            new SqlParameter[] {
                new SqlParameter("@WorkerID", worker.WorkerID),
                new SqlParameter("@FromPath", worker.FromPath),
                new SqlParameter("@ToPath", worker.ToPath),
                paramTrxNo,
                paramCreateDateTime
            });


            // Final return workerjob object.
            return new WorkerJobDto {
                TrxNo = Convert.ToInt32(paramTrxNo.Value),
                WorkerID = worker.WorkerID,
                FromPath = worker.FromPath,
                ToPath = worker.ToPath,
                CreateDateTime = Convert.ToDateTime(paramCreateDateTime.Value)
            };
        }


        /**
        * @dev Flag inactive workerjob.
        * @param workerId The key refer to TrxNo.
        */
        [Authorize]
        [HttpPost("inactive/{workerId}")]
        public async Task<ActionResult<bool>> inactiveWorkerJob(string workerId)
        {
            // 1. Create DbAdapter object for execute user to database.
            var adapter = new DbAdapter(_config.GetConnectionString("DefaultConnection"));

            // 2. Execute set inactive the worker.
            await adapter.executedAsync("SP_WorkerJob_InActivate", 
            CommandType.StoredProcedure,
            new SqlParameter[] {
                new SqlParameter("@WorkerID", workerId)
            });

            return Ok();
        }

    }



    /**
    * @notice The WorkerJobHelper class is responsible to convert stream data objects.
    * NOTE: It is a static class.
    */
    public static class WorkerJobHelper
    {
        public static List<WorkerJobDto> convertDataSetToWorkerJobs(DataSet dataset)
        {
            // 1. Create a list of WorkerJobDto.
            List<WorkerJobDto> jobs = new List<WorkerJobDto>();

            try
            {
                // 2. Convert data row to WorkerJobDto object.
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    // NOTE: Add new WorkerJobDto.
                    jobs.Add(new WorkerJobDto {
                        TrxNo = Convert.ToInt32(row["TrxNo"]),
                        WorkerID = row["WorkerID"].ToString(),
                        FromPath = row["FromPath"].ToString(),
                        ToPath = row["ToPath"].ToString(),
                        WorkerStatus = row["WorkerStatus"].ToString(),
                        CreateDateTime = Convert.ToDateTime(row["CreateDateTime"]),
                        UpdateDateTime = Convert.ToDateTime(row["UpdateDateTime"])
                    });

                }

            } catch {}

            // Final Return list of WorkerJobDto.
            return jobs;
        }
    }
}