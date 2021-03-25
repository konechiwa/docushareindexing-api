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
    public class DeclarationMessageController : BaseApiController
    {

        /**
        * @notice readonly variables.
        */
        private readonly IConfiguration _config;


        /**
        * @notice constructor class.
        */
        public DeclarationMessageController(IConfiguration config)
        {
            _config = config;
        }

        /**
        * @dev Return export shipment declarations data.
        * @param refno The variable refer to key in database.
        */
        [Authorize]
        [HttpGet]
        [Route("export/{refno}")]
        public async Task<ActionResult<IEnumerable<DeclarationMessageDto>>> getExportShipmentMessageByRefNo(string refno)
        {
            // 1. Initialize data adapter with connection string.
            DbAdapter adapter = new DbAdapter(_config.GetConnectionString("ExportDeclarationDbConnection"));
            
            // 2. Retrive data from database by refno.
            DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                "SP_DOCUSHARE_EXP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});
            
            // 3. Convert datarow to object representation.
            return DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);
        }



        /**
        * @dev Return import shipment declarations data.
        * @param refno The variable refer to key in database.
        */
        [Authorize]
        [HttpGet]
        [Route("import/{refno}")]
        public async Task<ActionResult<IEnumerable<DeclarationMessageDto>>> getImportShipmentMessageByRefNo(string refno)
        {
            // 1. Initialize data adapter with connection string.
            DbAdapter adapter = new DbAdapter(_config.GetConnectionString("ImportDeclarationDbConnection"));
            
            // 2. Retrive data from database by refno.
            DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                "SP_DOCUSHARE_IMP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});

            // 3. Convert datarow to object representation.
            return DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);
        }

        
    }

    public static class DeclarationMessageHelper 
    {
        /**
        * @dev Returns Collections of declarations
        * @param dataset The data row collection.
        * NOTE : The method will convert DataRow to object.
        */
        public static List<DeclarationMessageDto> convertDataSetToDeclarationMessage(DataSet dataset)
        {

            // 1. Initialize declaration message objects.
            List<DeclarationMessageDto> declarationMessages = new List<DeclarationMessageDto>();


            // 2. Do convert datarow to declarationmessage object. 
            //    Then add those objects to collection declaration.
            try
            {
                foreach (DataRow row in dataset.Tables[0].Rows)
                {
                    declarationMessages.Add(new DeclarationMessageDto 
                    {
                        METAPath = row["METAPath"].ToString(),
                        Title = row["Title"].ToString(),
                        ShipmentType = row["ShipmentType"].ToString(),
                        BranchCode = row["BranchCode"].ToString(),
                        RefNo = row["RefNo"].ToString(),
                        DecNo = row["DecNo"].ToString(),
                        CommercialInvoices = row["CommercialInvoices"].ToString(),
                        CmpTaxNo = row["CmpTaxNo"].ToString(),
                        CmpBranch = row["CmpBranch"].ToString(),
                        CmpName = row["CmpName"].ToString(),
                        VesselName = row["VesselName"].ToString(),
                        VoyNumber = row["VoyNumber"].ToString(),
                        MasterBL = row["MasterBL"].ToString(),
                        HouseBL = row["HouseBL"].ToString(),
                        DestCountry = row["DestCountry"].ToString(),
                        DeptCountry = row["DeptCountry"].ToString(),
                        ETA = Convert.ToDateTime(row["ETA"]),
                        ETD = Convert.ToDateTime(row["ETD"]),
                        UDateDeclare = Convert.ToDateTime(row["UDateDeclare"]),
                        UDateRelease = Convert.ToDateTime(row["UDateRelease"]),
                        MaterialType = row["MaterialType"].ToString(),
                        Period = row["Period"].ToString()
                    });
                }

            } catch {}

            // Final Return the objects.
            return declarationMessages;
        }
    }
}