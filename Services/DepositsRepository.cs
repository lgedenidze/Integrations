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
    public class DepositsRepository : IDepositsRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public DepositsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;

        }


        public async Task<GeneralResult> OpenDeposit(OpenDepositParams openDepositParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddDeposit
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.OPEN_DEPOSIT";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = openDepositParams.customerId;
                        cmd.Parameters.Add("p_deposit_type", OracleDbType.Varchar2).Value = openDepositParams.depositType;
                        cmd.Parameters.Add("p_deposit_subtype", OracleDbType.Varchar2).Value = openDepositParams.depositSubType;
                        cmd.Parameters.Add("p_ccy", OracleDbType.Varchar2).Value = openDepositParams.currency;
                        cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = openDepositParams.amount;
                        cmd.Parameters.Add("p_period_in_months", OracleDbType.Int16).Value = openDepositParams.periodInMonths == null ? DBNull.Value : Convert.ToInt16(openDepositParams.periodInMonths.Value);
                        cmd.Parameters.Add("p_account_id_to_transfer_from", OracleDbType.Varchar2).Value = openDepositParams.accountIdToTransferFrom;
                        cmd.Parameters.Add("p_capit_account_id", OracleDbType.Varchar2).Value = openDepositParams.capitalizationAccountId;
                        cmd.Parameters.Add("p_agreement_id", OracleDbType.Varchar2).Value = openDepositParams.agreementId;

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


        public async Task<GetDepositPreOpeningDetailsResult> GetDepositPreOpeningDetails(int   customerId,
                                                                                         string depositType,
                                                                                         string currency,
                                                                                         int    periodInMonths, 
                                                                                         string depositSubType)
        {
            GetDepositPreOpeningDetailsResult getDepositPreOpeningDetailsResult = null;

            using (OracleConnection oracleConn = new OracleConnection(connectionString))
            {
                #region GetLoanDetails
                using (OracleCommand cmd = oracleConn.CreateCommand())
                {
                    cmd.CommandText = "integrations.GET_DEPOSIT_PRE_OPEN_DETAILS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    // Adding parameters to the command
                    cmd.Parameters.Add("P_CUST_NO", OracleDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("P_DEPOSIT_TYPE", OracleDbType.Varchar2).Value = depositType;
                    cmd.Parameters.Add("P_CCY", OracleDbType.Varchar2).Value = currency;
                    cmd.Parameters.Add("P_PERIOD_IN_MONTHS", OracleDbType.Int32).Value = periodInMonths;
                    cmd.Parameters.Add("p_deposit_suB_type", OracleDbType.Varchar2).Value = depositSubType;
                    cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
              

                    // Open connection
                    if (oracleConn.State != ConnectionState.Open)
                    {
                        await oracleConn.OpenAsync();
                    }

                    using (DbDataReader oraReader = await cmd.ExecuteReaderAsync())
                    {
                        if (oraReader.HasRows)
                        {
                            getDepositPreOpeningDetailsResult = new GetDepositPreOpeningDetailsResult();

                            while (await oraReader.ReadAsync())
                            {
                                // Assuming your PL/SQL cursor returns a single row, mapping the data
                                getDepositPreOpeningDetailsResult.EffectiveRate = await oraReader.IsDBNullAsync("effective_Rate") ? null : oraReader.GetDecimal("effective_Rate");
                                getDepositPreOpeningDetailsResult.EffectiveRateInGel = await oraReader.IsDBNullAsync("effective_Rate_In_Gel") ? null : oraReader.GetDecimal("effective_Rate_In_Gel");
                                getDepositPreOpeningDetailsResult.InterestRate = await oraReader.IsDBNullAsync("interest_Rate") ? null : oraReader.GetDecimal("interest_Rate");
                                getDepositPreOpeningDetailsResult.EarlyClosureInterestRate = await oraReader.IsDBNullAsync("early_Closure_Interest_Rate") ? null : oraReader.GetDecimal("early_Closure_Interest_Rate");
                                getDepositPreOpeningDetailsResult.RateOnAmount = await oraReader.IsDBNullAsync("rate_On_Amount") ? null : oraReader.GetDecimal("rate_On_Amount");
                                getDepositPreOpeningDetailsResult.WithdrawalPrincipalCommission = await oraReader.IsDBNullAsync("withdrawal_Principal_Com") ? null : oraReader.GetDecimal("withdrawal_Principal_Com");
                                getDepositPreOpeningDetailsResult.InterestType = await oraReader.IsDBNullAsync("interest_Type") ? null : oraReader.GetString("interest_Type");
                                getDepositPreOpeningDetailsResult.InterestPeriod = await oraReader.IsDBNullAsync("interest_Period") ? null : oraReader.GetInt32("interest_Period");
                                getDepositPreOpeningDetailsResult.InterestPeriodType = await oraReader.IsDBNullAsync("interest_Period_Type") ? null : oraReader.GetString("interest_Period_Type");
                                getDepositPreOpeningDetailsResult.MinFirstAmount = await oraReader.IsDBNullAsync("min_First_Amount") ? null : oraReader.GetDecimal("min_First_Amount");
                                getDepositPreOpeningDetailsResult.MinimumMonthlyAdd = await oraReader.IsDBNullAsync("minimum_Monthly_Add") ? null : oraReader.GetDecimal("minimum_Monthly_Add");


                            }
                            await oraReader.CloseAsync();

                         }
                    }
                    await oracleConn.CloseAsync();
                    await oracleConn.DisposeAsync();
                }

                return getDepositPreOpeningDetailsResult;
                #endregion
            }
        }

    }
}
