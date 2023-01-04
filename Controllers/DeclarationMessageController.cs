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


        [Authorize]
        [HttpGet]
        [Route("expcomplete/{period}")]
        public async Task<ActionResult<IEnumerable<DeclarationMessageDto>>> getExportCompleteList(string period)
        {
            // 1. Initialize data adapter with connection string.
            DbAdapter adapter = new DbAdapter(_config.GetConnectionString("ExportDeclarationDbConnection"));
            DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));
            DbAdapter adapter3 = new DbAdapter(_config.GetConnectionString("DocushareConnection"));

            DataSet dsDecEDI= await adapter2.getDataSetAsync(
                "SP_PERIOD_EXP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] {
                    new SqlParameter("@DocNameType", "NMB"),
                    new SqlParameter("@Period", period)
                });
            
            DataSet dsDecDOC = await adapter3.getDataSetAsync(
                "SP_ListExpMessage_Select", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@Period", period)});

            if (dsDecEDI.Tables[0].Rows.Count > 0 && dsDecDOC.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow rowEDI in dsDecEDI.Tables[0].Rows)
                {
                    DataTable tableDOC = dsDecDOC.Tables[0].DefaultView.ToTable(true,String.Format("Export_DecNO = '{0}'",rowEDI["DecNo"].ToString()));
                    if(tableDOC.Rows.Count == 1)
                    {
                        rowEDI["DocName"] = tableDOC.Rows[0]["Export_DocName"];
                        rowEDI["ShipmentType"] = tableDOC.Rows[0]["Export_ShipmentType"];
                        rowEDI["RefNo"] = tableDOC.Rows[0]["Export_RefNo"];
                        rowEDI["DecNO"] = tableDOC.Rows[0]["Export_DecNO"];
                        rowEDI["CommercialInvoices"] = tableDOC.Rows[0]["Export_CommercialInvoices"];
                        rowEDI["MaterialType"] = tableDOC.Rows[0]["Export_MaterialType"];
                        rowEDI["Period"] = tableDOC.Rows[0]["Export_Period"];
                    }
                }
            }
            return DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDecEDI);
        }





        [Authorize]
        [HttpGet]
        [Route("exptransit/{refno}")]
        public async Task<ActionResult<IEnumerable<DeclarationMessageDto>>> getExportTransitMessageByRefNo(string refno)
        {
            // 1. Initialize data adapter with connection string.
            DbAdapter adapter = new DbAdapter(_config.GetConnectionString("TransitDbConnection"));
            
            // 2. Retrive data from database by refno.
            DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                "SP_TRANSIT_EXP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});
            
            // 3. Convert datarow to object representation.
            return DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);
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
            DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));
            
            // 2. Retrive data from database by refno.
            DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                "SP_DOCUSHARE_EXP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});

            if (dsDeclarationMessage.Tables[0].Rows.Count > 0)
            {
                DataSet dsClearDate = await adapter2.getDataSetAsync(
                "SP_CLEARDATE_EXP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});

                if (dsClearDate.Tables[0].Rows.Count > 0) dsDeclarationMessage.Tables[0].Rows[0]["Period"] = dsClearDate.Tables[0].Rows[0][0];
            }
            
            // 3. Convert datarow to object representation.
            return DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);
        }

        [Authorize]
        [HttpGet]
        [Route("imptransit/{refno}")]
        public async Task<ActionResult<IEnumerable<DeclarationMessageDto>>> getImportTransitMessageByRefNo(string refno)
        {
            // 1. Initialize data adapter with connection string.
            DbAdapter adapter = new DbAdapter(_config.GetConnectionString("TransitDbConnection"));;

            // 2. Retrive data from database by refno.
            DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                "SP_TRANSIT_IMP_FIND", 
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
            DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));

            // 2. Retrive data from database by refno.
            DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                "SP_DOCUSHARE_IMP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});

            if (dsDeclarationMessage.Tables[0].Rows.Count > 0)
            {
                DataSet dsClearDate = await adapter2.getDataSetAsync(
                "SP_CLEARDATE_IMP_FIND", 
                CommandType.StoredProcedure, 
                new SqlParameter[] { new SqlParameter("@REFNO", refno)});
 
                if (dsClearDate.Tables[0].Rows.Count > 0) dsDeclarationMessage.Tables[0].Rows[0]["Period"] = dsClearDate.Tables[0].Rows[0][0];
            }

            // 3. Convert datarow to object representation.
            return DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);
        }

        [Authorize]
        [HttpPut]
        [Route("export/{refno}")]
        public async Task<ActionResult<string>> updateExportShipmentMessageByRefNo()
        {
            int indexID = 0;
            string decNo = "";
            string refno = "";
            string textError = "";
            try
            {
                // 1. Initialize data adapter with connection string.
                DbAdapter adapter = new DbAdapter(_config.GetConnectionString("ExportDeclarationDbConnection"));
                DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));
                DbAdapter adapter3 = new DbAdapter(_config.GetConnectionString("DocushareConnection"));

                // 2. Retrive data from database by refno.
                DataSet dsDecNoMessage = await adapter3.getDataSetAsync("SP_IndexExpMessage_Select", CommandType.StoredProcedure);
                
                foreach (DataRow row in dsDecNoMessage.Tables[0].Rows)
                {
                    refno = row[0].ToString();

                    DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                    "SP_DOCUSHARE_EXP_FIND", 
                    CommandType.StoredProcedure, 
                    new SqlParameter[] { new SqlParameter("@REFNO", refno)});

                    if (dsDeclarationMessage.Tables[0].Rows.Count > 0)
                    {
                        DataSet dsClearDate = await adapter2.getDataSetAsync(
                        "SP_CLEARDATE_EXP_FIND", 
                        CommandType.StoredProcedure, 
                        new SqlParameter[] { new SqlParameter("@REFNO", refno)});

                        if (dsClearDate.Tables[0].Rows.Count > 0) dsDeclarationMessage.Tables[0].Rows[0]["Period"] = dsClearDate.Tables[0].Rows[0][0];

                        List<DeclarationMessageDto> declarationMessages = new List<DeclarationMessageDto>();
                        declarationMessages = DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);

                        // 2. Create SqlParameter object for output.
                        SqlParameter paramIndexID = new SqlParameter("@IndexID", SqlDbType.Int);
                        paramIndexID.Direction = ParameterDirection.Output;

                        // 3. Executing SqlCommand to database.
                        await adapter3.executedAsync("SP_IndexExpMessage_Update", 
                            CommandType.StoredProcedure, 
                            new SqlParameter[] {
                                // new SqlParameter("@METAPath", declarationMessages[0].METAPath),
                                // new SqlParameter("@Title", declarationMessages[0].Title),
                                new SqlParameter("@ShipmentType", declarationMessages[0].ShipmentType),
                                new SqlParameter("@BranchCode", declarationMessages[0].BranchCode),
                                new SqlParameter("@RefNo", declarationMessages[0].RefNo),
                                new SqlParameter("@DecNo", declarationMessages[0].DecNo),
                                new SqlParameter("@CommercialInvoices", declarationMessages[0].CommercialInvoices),
                                new SqlParameter("@CmpTaxNo", declarationMessages[0].CmpTaxNo),
                                new SqlParameter("@CmpBranch", declarationMessages[0].CmpBranch),
                                new SqlParameter("@CmpName", declarationMessages[0].CmpName),
                                new SqlParameter("@VesselName", declarationMessages[0].VesselName),
                                new SqlParameter("@VoyNumber", declarationMessages[0].VoyNumber),
                                new SqlParameter("@MasterBL", declarationMessages[0].MasterBL),
                                new SqlParameter("@HouseBL", declarationMessages[0].HouseBL),
                                new SqlParameter("@DestCountry", declarationMessages[0].DestCountry),
                                new SqlParameter("@DeptCountry", declarationMessages[0].DeptCountry),
                                new SqlParameter("@MaterialType", declarationMessages[0].MaterialType),
                                new SqlParameter("@ETA", declarationMessages[0].ETA),
                                new SqlParameter("@ETD", declarationMessages[0].ETD),
                                new SqlParameter("@UDateDeclare", declarationMessages[0].UDateDeclare),
                                new SqlParameter("@UDateRelease", declarationMessages[0].UDateRelease),
                                // new SqlParameter("@DatabaseSeq", declarationMessages[0].DatabaseSeq),
                                // new SqlParameter("@BranchSeq", declarationMessages[0].BranchSeq),
                                new SqlParameter("@Period", declarationMessages[0].Period),
                                paramIndexID
                            });

                            // 4. Convert output value from SqlParameter to variable.
                            indexID = Convert.ToInt32(paramIndexID.Value);
                            decNo = declarationMessages[0].DecNo;
                    }else{
                        // 2. Create SqlParameter object for output.
                        SqlParameter paramIndexID = new SqlParameter("@IndexID", SqlDbType.Int);
                        paramIndexID.Direction = ParameterDirection.Output;

                        // 3. Executing SqlCommand to database.
                        await adapter3.executedAsync("SP_IndexExpMessageNotFound_Update", 
                            CommandType.StoredProcedure, 
                            new SqlParameter[] {
                                new SqlParameter("@DecNo", refno),
                                paramIndexID
                            });

                            // 4. Convert output value from SqlParameter to variable.
                            indexID = Convert.ToInt32(paramIndexID.Value);
                            decNo = refno;
                    }
                }
            }
            catch (Exception ex)
            {
                textError = " : " + ex.Message;
            }
            
            // 3. Convert datarow to object representation.
            return indexID + " : " + decNo + textError;
        }

        [Authorize]
        [HttpPut]
        [Route("import/{refno}")]
        public async Task<ActionResult<string>> updateImportShipmentMessageByRefNo()
        {
            int indexID = 0;
            string decNo = "";
            string refno = "";
            string textError = "";
            try
            {
                // 1. Initialize data adapter with connection string.
                DbAdapter adapter = new DbAdapter(_config.GetConnectionString("ImportDeclarationDbConnection"));
                DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));
                DbAdapter adapter3 = new DbAdapter(_config.GetConnectionString("DocushareConnection"));

                // 2. Retrive data from database by refno.
                DataSet dsDecNoMessage = await adapter3.getDataSetAsync("SP_IndexImpMessage_Select", CommandType.StoredProcedure);

                foreach (DataRow row in dsDecNoMessage.Tables[0].Rows)
                {
                    refno = row[0].ToString();

                    DataSet dsDeclarationMessage = await adapter.getDataSetAsync(
                    "SP_DOCUSHARE_IMP_FIND", 
                    CommandType.StoredProcedure, 
                    new SqlParameter[] { new SqlParameter("@REFNO", refno)});

                    if (dsDeclarationMessage.Tables[0].Rows.Count > 0)
                    {
                        DataSet dsClearDate = await adapter2.getDataSetAsync(
                        "SP_CLEARDATE_IMP_FIND", 
                        CommandType.StoredProcedure, 
                        new SqlParameter[] { new SqlParameter("@REFNO", refno)});
        
                        if (dsClearDate.Tables[0].Rows.Count > 0) dsDeclarationMessage.Tables[0].Rows[0]["Period"] = dsClearDate.Tables[0].Rows[0][0];

                        List<DeclarationMessageDto> declarationMessages = new List<DeclarationMessageDto>();
                        declarationMessages = DeclarationMessageHelper.convertDataSetToDeclarationMessage(dsDeclarationMessage);

                        // 2. Create SqlParameter object for output.
                        SqlParameter paramIndexID = new SqlParameter("@IndexID", SqlDbType.Int);
                        paramIndexID.Direction = ParameterDirection.Output;

                        // 3. Executing SqlCommand to database.
                        await adapter3.executedAsync("SP_IndexImpMessage_Update", 
                            CommandType.StoredProcedure, 
                            new SqlParameter[] {
                                // new SqlParameter("@METAPath", declarationMessages[0].METAPath),
                                // new SqlParameter("@Title", declarationMessages[0].Title),
                                new SqlParameter("@ShipmentType", declarationMessages[0].ShipmentType),
                                new SqlParameter("@BranchCode", declarationMessages[0].BranchCode),
                                new SqlParameter("@RefNo", declarationMessages[0].RefNo),
                                new SqlParameter("@DecNo", declarationMessages[0].DecNo),
                                new SqlParameter("@CommercialInvoices", declarationMessages[0].CommercialInvoices),
                                new SqlParameter("@CmpTaxNo", declarationMessages[0].CmpTaxNo),
                                new SqlParameter("@CmpBranch", declarationMessages[0].CmpBranch),
                                new SqlParameter("@CmpName", declarationMessages[0].CmpName),
                                new SqlParameter("@VesselName", declarationMessages[0].VesselName),
                                new SqlParameter("@VoyNumber", declarationMessages[0].VoyNumber),
                                new SqlParameter("@MasterBL", declarationMessages[0].MasterBL),
                                new SqlParameter("@HouseBL", declarationMessages[0].HouseBL),
                                new SqlParameter("@DestCountry", declarationMessages[0].DestCountry),
                                new SqlParameter("@DeptCountry", declarationMessages[0].DeptCountry),
                                new SqlParameter("@MaterialType", declarationMessages[0].MaterialType),
                                new SqlParameter("@ETA", declarationMessages[0].ETA),
                                new SqlParameter("@ETD", declarationMessages[0].ETD),
                                new SqlParameter("@UDateDeclare", declarationMessages[0].UDateDeclare),
                                new SqlParameter("@UDateRelease", declarationMessages[0].UDateRelease),
                                // new SqlParameter("@DatabaseSeq", declarationMessages[0].DatabaseSeq),
                                // new SqlParameter("@BranchSeq", declarationMessages[0].BranchSeq),
                                new SqlParameter("@Period", declarationMessages[0].Period),
                                paramIndexID
                            });
                        
                        // 4. Convert output value from SqlParameter to variable.
                        indexID = Convert.ToInt32(paramIndexID.Value);
                        decNo = declarationMessages[0].DecNo;
                    }else{
                        // 2. Create SqlParameter object for output.
                        SqlParameter paramIndexID = new SqlParameter("@IndexID", SqlDbType.Int);
                        paramIndexID.Direction = ParameterDirection.Output;

                        // 3. Executing SqlCommand to database.
                        await adapter3.executedAsync("SP_IndexImpMessageNotFound_Update", 
                            CommandType.StoredProcedure, 
                            new SqlParameter[] {
                                new SqlParameter("@DecNo", refno),
                                paramIndexID
                            });
                        
                        // 4. Convert output value from SqlParameter to variable.
                        indexID = Convert.ToInt32(paramIndexID.Value);
                        decNo = refno;
                    }

                }
                
            }
            catch (Exception ex)
            {
                textError = " : " + ex.Message;
            }

            // 3. Convert datarow to object representation.
            return indexID + " : " + decNo + textError;
        }

        [Authorize]
        [HttpPut]
        [Route("experiod/{refno}")]
        public async Task<ActionResult<string>> updateExportPeriod()
        {
            string decNo = "";
            string period = ""; 
            string textError = "";
            try
            {
                // 1. Initialize data adapter with connection string.
                DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));
                DbAdapter adapter3 = new DbAdapter(_config.GetConnectionString("DocushareConnection"));

                // 2. Retrive data from database by refno.
                DataSet dsDecNoMessage = await adapter3.getDataSetAsync("SP_IndexExpPeriod_Select", CommandType.StoredProcedure);
                
                foreach (DataRow row in dsDecNoMessage.Tables[0].Rows)
                {
                    decNo = row[0].ToString();

                    DataSet dsClearDate = await adapter2.getDataSetAsync(
                    "SP_CLEARDATE_EXP_FIND", 
                    CommandType.StoredProcedure, 
                    new SqlParameter[] { new SqlParameter("@REFNO", decNo)});

                    period = dsClearDate.Tables[0].Rows[0][0].ToString();

                    // 3. Executing SqlCommand to database.
                    await adapter3.executedAsync("SP_IndexExpPeriod_Update", 
                        CommandType.StoredProcedure, 
                        new SqlParameter[] {
                            new SqlParameter("@DecNo", decNo),
                            new SqlParameter("@Period", period)
                        });
                }
            }
            catch (Exception ex)
            {
                textError = " : " + ex.Message;
            }
            
            // 3. Convert datarow to object representation.
            return decNo + textError;
        }

        [Authorize]
        [HttpPut]
        [Route("imperiod/{refno}")]
        public async Task<ActionResult<string>> updateImportPeriod()
        {
            string decNo = "";
            string period = ""; 
            string textError = "";
            try
            {
                // 1. Initialize data adapter with connection string.
                DbAdapter adapter2 = new DbAdapter(_config.GetConnectionString("Edidata1Connection"));
                DbAdapter adapter3 = new DbAdapter(_config.GetConnectionString("DocushareConnection"));

                // 2. Retrive data from database by refno.
                DataSet dsDecNoMessage = await adapter3.getDataSetAsync("SP_IndexImpPeriod_Select", CommandType.StoredProcedure);
                
                foreach (DataRow row in dsDecNoMessage.Tables[0].Rows)
                {
                    decNo = row[0].ToString();

                    DataSet dsClearDate = await adapter2.getDataSetAsync(
                    "SP_CLEARDATE_IMP_FIND", 
                    CommandType.StoredProcedure, 
                    new SqlParameter[] { new SqlParameter("@REFNO", decNo)});

                    period = dsClearDate.Tables[0].Rows[0][0].ToString();

                    // 3. Executing SqlCommand to database.
                    await adapter3.executedAsync("SP_IndexImpPeriod_Update", 
                        CommandType.StoredProcedure, 
                        new SqlParameter[] {
                            new SqlParameter("@DecNo", decNo),
                            new SqlParameter("@Period", period)
                        });
                }
                
            }
            catch (Exception ex)
            {
                textError = " : " + ex.Message;
            }

            // 3. Convert datarow to object representation.
            return decNo + textError;
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
                        DatabaseSeq = row["DatabaseSeq"].ToString(),
                        BranchSeq = row["BranchSeq"].ToString(),
                        Period = row["Period"].ToString()
                    });
                }

            } catch {}

            // Final Return the objects.
            return declarationMessages;
        }
    }
}