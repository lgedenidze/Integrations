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


        public async Task<GeneralResult> ResetPin(ResetCardPinParams resetCardPinParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.RESET_CARD_PIN";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = resetCardPinParams.customerId;
                        cmd.Parameters.Add("p_card_id", OracleDbType.Int32).Value = resetCardPinParams.cardId;
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

        public async Task<GeneralResult> Block(BlockCardParams blockCardParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.BLOCK_CARD";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = blockCardParams.customerId;
                        cmd.Parameters.Add("p_card_id", OracleDbType.Int32).Value = blockCardParams.cardId;
                        cmd.Parameters.Add("p_block_type", OracleDbType.Varchar2).Value = blockCardParams.cardBlockType.ToString();

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

        public async Task<GeneralResult> Unblock(UnblockCardParams unblockCardParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.UNBLOCK_CARD";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = unblockCardParams.customerId;
                        cmd.Parameters.Add("p_card_id", OracleDbType.Int32).Value = unblockCardParams.cardId;

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

        public async Task<GeneralResult> ChangeCurrencyPriority(ChangeCardCurrencyPriorityParams changeCardCurrencyPriorityParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.CHANGE_CARD_CCY_PRIORITY";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = changeCardCurrencyPriorityParams.customerId;
                        cmd.Parameters.Add("p_card_id", OracleDbType.Int32).Value = changeCardCurrencyPriorityParams.cardId;
                        cmd.Parameters.Add("p_ccy_list", OracleDbType.Varchar2).Value = String.Join(",", changeCardCurrencyPriorityParams.currencies.Select(x => x.currency));
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

        public async Task<GeneralResult> ChangeHighRiskTransactionStatus(ChangeCardHighRiskTransactionStatusParams changeCardHighRiskTransactionStatusParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.CHANGE_CARD_HIGH_RISK_TRN_STS";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = changeCardHighRiskTransactionStatusParams.customerId;
                        cmd.Parameters.Add("p_card_id", OracleDbType.Int32).Value = changeCardHighRiskTransactionStatusParams.cardId;
                        cmd.Parameters.Add("p_type", OracleDbType.Varchar2).Value = changeCardHighRiskTransactionStatusParams.cardHighRiskTransactionStatus.ToString();

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


        public async Task<GeneralResult> LinkInstantCardToAccount(LinkInstantCardToAccountParams linkInstantCardToAccountParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.link_instant_card_to_account";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_account_id", OracleDbType.Varchar2).Value = linkInstantCardToAccountParams.AccountId;
                        cmd.Parameters.Add("p_last_four", OracleDbType.Varchar2).Value = linkInstantCardToAccountParams.CardLastFourDigits; 
                        cmd.Parameters.Add("p_emboss_Name", OracleDbType.Varchar2).Value = linkInstantCardToAccountParams.EmbossName;
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
