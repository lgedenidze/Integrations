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
    public class TransactionsRepository : ITransactionsRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;
        public TransactionsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }

        public async Task<TransferMoneyResult> TransferMoney(TransferMoneyData transferMoneyData)
        {
            TransferMoneyResult v_result = null;
            bool v_inserted = await InsertOrder(transferMoneyData);
            if (v_inserted)
            {
                v_result = await ProceedOrder(transferMoneyData.transactions.Select(x => x.Id).ToList());
            }
            else
            {
                Error v_error = new Error { errorCode = -1, errorMessageGe = "ვერ მოხერხდა მონაცემების შენახვა", errorMessageEn = "Error while inserting order" };
                v_result = new TransferMoneyResult { success = false, error = v_error };
            }


            return v_result;
        }

        private async Task<bool> InsertOrder(TransferMoneyData transferMoneyData)
        {
            bool v_result = false;
            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                if (oracle_conn.State != ConnectionState.Open)
                {
                    await oracle_conn.OpenAsync();
                }
                #region InsertOrder
                using (OracleTransaction transaction = oracle_conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in transferMoneyData.transactions)
                        {
                            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.INSERT_TRANSACTION_ORDER", oracle_conn))
                            {
                                cmd.Transaction = transaction;
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.BindByName = true;
                                if (_commandTimeout != null)
                                    cmd.CommandTimeout = _commandTimeout.Value;

                                cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                                cmd.Parameters.Add("p_id", OracleDbType.Varchar2).Value = item.Id;
                                cmd.Parameters.Add("p_transfer_type", OracleDbType.Varchar2).Value = item.TransferType.ToString();
                                cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = item.Amount;
                                cmd.Parameters.Add("p_currency", OracleDbType.Varchar2).Value = item.Currency;
                                cmd.Parameters.Add("p_amount_exch", OracleDbType.Decimal).Value = item.Exchange == null ? DBNull.Value : item.Exchange.Amount;
                                cmd.Parameters.Add("p_currency_exch", OracleDbType.Varchar2).Value = item.Exchange == null ? DBNull.Value : item.Exchange.Currency;
                                cmd.Parameters.Add("p_currency_exch_fix", OracleDbType.Varchar2).Value = item.Exchange == null ? DBNull.Value : item.Exchange.CurrencyFix;
                                cmd.Parameters.Add("p_customer_id", OracleDbType.Int32).Value = (item.Sender == null ? DBNull.Value : (item.Sender.CustomerId == null ? DBNull.Value : item.Sender.CustomerId.Value ));
                                cmd.Parameters.Add("p_account_id", OracleDbType.Varchar2).Value = (item.Sender == null ? DBNull.Value : item.Sender.AccountId);
                                cmd.Parameters.Add("p_receiver_bank_code", OracleDbType.Varchar2).Value = item.ReceiverBank == null ? DBNull.Value : item.ReceiverBank.BankCode;
                                cmd.Parameters.Add("p_receiver_bank_name", OracleDbType.Varchar2).Value = item.ReceiverBank == null ? DBNull.Value : item.ReceiverBank.BankName;
                                cmd.Parameters.Add("p_intermediate_bank_code", OracleDbType.Varchar2).Value = item.IntermediateBank == null ? DBNull.Value : item.IntermediateBank.BankCode;
                                cmd.Parameters.Add("p_intermediate_bank_name", OracleDbType.Varchar2).Value = item.IntermediateBank == null ? DBNull.Value : item.IntermediateBank.BankName;
                                cmd.Parameters.Add("p_receiver_account_id", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : item.Receiver.CreditAccountId;
                                cmd.Parameters.Add("p_receiver_name", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : item.Receiver.Name;
                                cmd.Parameters.Add("p_receiver_address", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : item.Receiver.Address;
                                cmd.Parameters.Add("p_receiver_country", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : item.Receiver.Country;
                                cmd.Parameters.Add("p_receiver_city", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : item.Receiver.City;

                                cmd.Parameters.Add("p_treasury_code", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : (item.Receiver.Treasury == null ? DBNull.Value : item.Receiver.Treasury.TreasuryCode);
                                cmd.Parameters.Add("p_tax_payer_code", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : (item.Receiver.Treasury == null ? DBNull.Value : item.Receiver.Treasury.TaxPayerCode);
                                cmd.Parameters.Add("p_tax_payer_name", OracleDbType.Varchar2).Value = item.Receiver == null ? DBNull.Value : (item.Receiver.Treasury == null ? DBNull.Value : item.Receiver.Treasury.TaxPayerName);
                                cmd.Parameters.Add("p_charges", OracleDbType.Varchar2).Value = item.Charges == null ? DBNull.Value : item.Charges.ToString();
                                cmd.Parameters.Add("p_payment_purpose", OracleDbType.Varchar2).Value = item.PaymentPurpose;
                                cmd.Parameters.Add("p_remarks", OracleDbType.Varchar2).Value = item.Remarks;
                                cmd.Parameters.Add("p_is_stp", OracleDbType.Int16).Value = item.IsSTP == null ? DBNull.Value : Convert.ToInt16(item.IsSTP.Value);

                                cmd.Parameters.Add("p_group_id", OracleDbType.Varchar2).Value = item.GroupId;
                                cmd.Parameters.Add("p_creator_user", OracleDbType.Varchar2).Value = item.CreatorUser;
                                cmd.Parameters.Add("p_channel", OracleDbType.Varchar2).Value = item.Channel;

                                cmd.Parameters.Add("p_remittance_name", OracleDbType.Varchar2).Value = item.Remittance == null ? DBNull.Value : item.Remittance.RemittanceName;
                                cmd.Parameters.Add("p_remittance_transfer_code", OracleDbType.Varchar2).Value = item.Remittance == null ? DBNull.Value : item.Remittance.RemittanceTransferCode;
                                cmd.Parameters.Add("p_remittance_counterparty", OracleDbType.Varchar2).Value = item.Remittance == null ? DBNull.Value : item.Remittance.RemittanceCounterparty;
                                cmd.Parameters.Add("p_remittance_country_code", OracleDbType.Varchar2).Value = item.Remittance == null ? DBNull.Value : item.Remittance.RemittanceCounterpartyCountryCode;
                                cmd.Parameters.Add("p_remittance_send_third_party", OracleDbType.Int16).Value = item.Remittance == null ? DBNull.Value : (item.Remittance.SendToThirdParty == null ? DBNull.Value : Convert.ToInt16(item.Remittance.SendToThirdParty.Value));

                                cmd.Parameters.Add("p_person_first_name", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.FirstNameGe;
                                cmd.Parameters.Add("p_person_first_name_en", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.FirstNameEn;
                                cmd.Parameters.Add("p_person_last_name", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.LastNameGe;
                                cmd.Parameters.Add("p_person_last_name_en", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.LastNameEn;
                                cmd.Parameters.Add("p_person_personal_no", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.PersonalNo;
                                cmd.Parameters.Add("p_person_birth_date", OracleDbType.Date).Value = item.Person == null ? DBNull.Value : item.Person.BirthDate;
                                cmd.Parameters.Add("p_person_sex", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : (item.Person.Sex == null ? DBNull.Value : item.Person.Sex.ToString());
                                cmd.Parameters.Add("p_person_citizenship", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.Citizenship;
                                cmd.Parameters.Add("p_person_address", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.Address;
                                cmd.Parameters.Add("p_person_phone", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : item.Person.Phone;

                                cmd.Parameters.Add("p_person_doc_type", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : (item.Person.Document == null ? DBNull.Value : item.Person.Document.DocumentType.ToString());
                                cmd.Parameters.Add("p_person_doc_no", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : (item.Person.Document == null ? DBNull.Value : item.Person.Document.DocumentNo);
                                cmd.Parameters.Add("p_person_doc_issue_date", OracleDbType.Date).Value = item.Person == null ? DBNull.Value : (item.Person.Document == null ? DBNull.Value : item.Person.Document.DocumentIssueDate);
                                cmd.Parameters.Add("p_person_doc_expire_date", OracleDbType.Date).Value = item.Person == null ? DBNull.Value : (item.Person.Document == null ? DBNull.Value : (item.Person.Document.DocumentExpireDate == null ? DBNull.Value : item.Person.Document.DocumentExpireDate.Value));
                                cmd.Parameters.Add("p_person_doc_issuer", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : (item.Person.Document == null ? DBNull.Value : item.Person.Document.DocumentIssuer);
                                cmd.Parameters.Add("p_person_doc_issuer_country", OracleDbType.Varchar2).Value = item.Person == null ? DBNull.Value : (item.Person.Document == null ? DBNull.Value : item.Person.Document.DocumentIssuerCountry);

                                cmd.Parameters.Add("p_do_not_auhtorize_fully", OracleDbType.Int16).Value = item.DoNotAuthorizeFully == null ? DBNull.Value : Convert.ToInt16(item.DoNotAuthorizeFully.Value);
                                cmd.Parameters.Add("p_transfer_fee", OracleDbType.Decimal).Value = item.TransferFee == null ? DBNull.Value : item.TransferFee.Value;



                                await cmd.ExecuteNonQueryAsync();

                                bool v_success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));
                                v_result = v_success;
                                if (!v_result)
                                    throw new Exception("insert failed");
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception exc)
                    {
                        v_result = false;
                        transaction.Rollback();
                    }
                }

   
                #endregion
            }


            return v_result;
        }

        private async Task<TransferMoneyResult> ProceedOrder(List<string> orderIds)
        {
            string v_order_ids_comma_separated = String.Join(",", orderIds);

            TransferMoneyResult v_result = null;
            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region ProceedOrder
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.PROCEED_TRANSACTION_ORDER";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("P_ORDER_ID_LIST", OracleDbType.Varchar2).Value = v_order_ids_comma_separated;
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

            return v_result;
        }


        public async Task<GetTransferFeeResult> GetTransferFee(string transferType, string debitAccountId, string creditAccountId, bool isStp, decimal amount)
        {
            GetTransferFeeResult v_result = new GetTransferFeeResult();
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region GetTransferFee
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.GET_TRANSFER_FEE";
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_transfer_type", OracleDbType.Varchar2).Value = transferType;
                        cmd.Parameters.Add("p_debet_acc", OracleDbType.Varchar2).Value = debitAccountId;
                        cmd.Parameters.Add("p_credit_acc", OracleDbType.Varchar2).Value = creditAccountId;
                        cmd.Parameters.Add("p_is_stp", OracleDbType.Int16).Value = Convert.ToInt16(isStp);
                        cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;


                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();

                        var v_return = (OracleDecimal)cmd.Parameters["return_value"].Value;
                        v_result.fee = v_return.IsNull ? null : (double?)v_return;

                        await oracle_conn.CloseAsync();
                        await oracle_conn.DisposeAsync();

                    }
                    #endregion
                }

            }
            catch (Exception exc)
            {
                v_result.error = new Error { errorCode = -1, errorMessageGe = exc.Message, errorMessageEn = exc.Message };
            }

            return v_result;
        }


        public async Task<GetTreasuryCodeDescrResult> GetTreasuryCodeDescr(string treasuryCode)
        {
            GetTreasuryCodeDescrResult v_result = new GetTreasuryCodeDescrResult();
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region GetTreasuryCodeDescr
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.GET_TREASURY_CODE_DESCR";
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Varchar2,400).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_treasury_code", OracleDbType.Varchar2,9).Value = treasuryCode;


                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();

                        var v_return =(OracleString)cmd.Parameters["return_value"].Value;

                        // Check if the return value is null

                        v_result.description = v_return.IsNull ? null : (String)v_return;


                        await oracle_conn.CloseAsync();
                        await oracle_conn.DisposeAsync();

                    }
                    #endregion
                }

            }
            catch (Exception exc)
            {
                v_result.error = new Error { errorCode = -1, errorMessageGe = exc.Message, errorMessageEn = exc.Message };
            }

            return v_result;
        }


        public async Task<GeneralResult> Authorize(AuthorizeParams authorizeParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.AUTHORIZE_TRANSACTION";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_order_id", OracleDbType.Varchar2).Value = authorizeParams.orderId;
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
