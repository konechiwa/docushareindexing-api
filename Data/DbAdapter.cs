using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace DocuShareIndexingAPI.Data
{
    public class DbAdapter
    {
        /**
        * @notice readonly variables.
        */
        private readonly string _sqlConnectionString;


        /**
        * @dev the constructor class.
        */
        public DbAdapter(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }



        /**
        * @dev Returns the asynconouse function of DataSet.
        * @param commandText Sets sql statement to execute.
        * @param commandType Sets SqlCommandType to execute.
        * @param parameters Sets SqlParameters to execute.
        */
        public Task<DataSet> getDataSetAsync(
            string commandText,
            CommandType commandType,
            params SqlParameter[] parameters)
        {
            return Task.Run(() =>
            {

                try
                {
                    using (var connection = new SqlConnection(_sqlConnectionString))
                    {
                        // 1. Open sql connection.
                        connection.Open();

                        // 2. Create SqlCommand to execute.
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = commandText;
                        cmd.CommandType = commandType;

                        // 3. Sets SqlParameter to SqlCommand.
                        if (parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);


                        // 4. Create SqlDataAdapter to get data from execute.
                        var adapter = new SqlDataAdapter(cmd);
                        var dsSet = new DataSet();
                        adapter.Fill(dsSet);


                        // 5. Returns DataSet object.
                        return dsSet;
                    }
                }
                catch (Exception ex){ 

                    // Throw error when catching exception.
                    throw new Exception(string.Format("getDataSetAsync : {0}", ex.Message));
                }
            });

        }

        /**
        * @dev Returns the asynconouse function of execution.
        * @param commandText Sets sql statement to execute.
        * @param commandType Sets SqlCommandType to execute.
        * @param parameters Sets SqlParameters to execute.
        */
        public Task<bool> executedAsync(
            string commandText,
            CommandType commandType,
            params SqlParameter[] parameters)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var connection = new SqlConnection(_sqlConnectionString))
                    {
                        // 1. Open sql connection.
                        connection.Open();

                        // 2. Create SqlCommand to execute.
                        var cmd = connection.CreateCommand();
                        cmd.CommandText = commandText;
                        cmd.CommandType = commandType;

                        // 3. Sets SqlParameter to SqlCommand.
                        if (parameters.Length > 0)
                            cmd.Parameters.AddRange(parameters);

                        // 4. SqlCommand to execute.
                        cmd.ExecuteNonQuery();

                        // Final boolean.
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Throw error when catching exception.
                    throw new Exception(string.Format("executedAsync : {0}", ex.Message));
                }
            });

        }
        
    }
}