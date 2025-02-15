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
using static Integrations.Model.Enums.BalanceType;

namespace Integrations.Services
{
    public class AccountsRepository : IAccountsRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public AccountsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }

        public async Task<Accounts> GetAccounts(int customerId)
        {
            Accounts v_accounts = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region get_accounts
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.GET_CUSTOMER_ACCOUNTS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("P_CUSTOMER_ID", OracleDbType.Int32).Value = customerId;
                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                    if (ora_reader.HasRows)
                    {
                        v_accounts = new Accounts();
                        v_accounts.accounts = new();
                        while (await ora_reader.ReadAsync())
                        {
                            Account v_account = new Account();
                            v_account.id = ora_reader.GetString("id");
                            v_account.iban = ora_reader.GetString("iban");
                            v_account.currency = ora_reader.GetString("currency");
                            v_account.isCard = Convert.ToBoolean(ora_reader.GetInt16("is_card"));
                            v_account.accountType = ora_reader.GetString("account_type");
                            v_account.debitEnabled = Convert.ToBoolean(ora_reader.GetInt16("debit_enabled"));
                            v_account.creditEnabled = Convert.ToBoolean(ora_reader.GetInt16("credit_enabled"));


                            v_accounts.accounts.Add(v_account);
                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
            }
       

            return v_accounts;
        }

        public async Task<GeneralResult> SendBalanceValidationToRabbitMQ(string accountId)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.SEND_BAL_VAL_INTO_REBBITMQ";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                       
                      
                        cmd.Parameters.Add("P_ACCOUNT_ID", OracleDbType.Varchar2).Value =accountId;

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

        public async Task<GetBalanceResult> GetBalance(string accountId )
        {
            GetBalanceResult v_result = new GetBalanceResult();
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region GetBalance
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.GET_BALANCE_ON_ACCOUNT";
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("P_ACCOUNT_ID", OracleDbType.Varchar2).Value = accountId;
 

                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();

                        var v_return = (OracleDecimal)cmd.Parameters["return_value"].Value;
                        v_result.balance = v_return.IsNull ? null : (double?)v_return;

                        await oracle_conn.CloseAsync();
                        await oracle_conn.DisposeAsync();

                    }
                    #endregion
                }

            }
            catch (Exception exc)
            {
                v_result.error = new Error { errorCode = -1, errorMessageGe =  exc.Message, errorMessageEn = exc.Message };
            }

            return v_result;
        }

 

 

    }
}
