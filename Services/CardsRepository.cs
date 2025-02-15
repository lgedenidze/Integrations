using Integrations.Model;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class CardsRepository : ICardsRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public CardsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;

        }


 

        public async Task<GeneralResult> AddCard(AddCardParams addCardParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCard
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.ADD_CARD";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = addCardParams.customerId;
                        cmd.Parameters.Add("p_card_product_type", OracleDbType.Varchar2).Value = addCardParams.cardProductType;
                        cmd.Parameters.Add("p_iban", OracleDbType.Varchar2).Value = addCardParams.iban;
                        cmd.Parameters.Add("p_ccy_list", OracleDbType.Varchar2).Value = String.Join(",", addCardParams.currencies.Select(x => x.currency));
                        cmd.Parameters.Add("p_card_holder", OracleDbType.Varchar2).Value = addCardParams.cardHolder;
                        cmd.Parameters.Add("p_give_out_branch_code", OracleDbType.Varchar2).Value = addCardParams.giveOutBranchCode;

                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();
                        bool v_success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));
                        v_result = new();
                        v_result.success = v_success;
                        if (!v_success)
                        {
                            var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();
                            if (ora_reader.HasRows)
                            {
                                while (await ora_reader.ReadAsync())
                                {
                                    v_result.error = new();
                                    v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                    v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                    v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");

                                }
                            }
                            await ora_reader.CloseAsync();
                        }

                        await oracle_conn.CloseAsync();
                        await oracle_conn.DisposeAsync();

                    }
                    #endregion
                }
            }
            catch (Exception exc)
            {
                v_result = new();
                v_result.success = false;
                v_result.error = new();
                v_result.error.errorCode = -1;
                v_result.error.errorMessageGe = "ზოგადი შეცდომა სერვისის გამოძახებისას";
                v_result.error.errorMessageEn = String.Format("General error. Error details: {0}", exc.Message + exc.StackTrace);
            }

            return v_result;

        }

 


    }
}
