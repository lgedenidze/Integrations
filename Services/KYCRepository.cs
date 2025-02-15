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
    public class KYCRepository : IKYCRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public KYCRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }


        public async Task<KYCResult> UpdateKyc(KYC kyc)
        {
            KYCResult v_result = new();
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    using (OracleTransaction transaction = oracle_conn.BeginTransaction())
                    {
                        try
                        {
                            KYCResult v_update_kyc_main_info_result =  await UpdateKycMainInfo(kyc, oracle_conn, transaction);
                            if(!v_update_kyc_main_info_result.success)
                            {
                                return v_update_kyc_main_info_result;
                            }


                            await DeleteKycEmployerInfo(v_update_kyc_main_info_result.kycId.Value, kyc.employerInfoList, oracle_conn, transaction);

                            if (kyc.employerInfoList?.Count > 0)
                            {                               
                                var v_update_kyc_emp_info_result = await UpdateKycEmployerInfo(v_update_kyc_main_info_result.kycId.Value, kyc.employerInfoList, oracle_conn, transaction);
                                if (!v_update_kyc_emp_info_result.success)
                                {
                                    return v_update_kyc_emp_info_result;
                                }
                            }

                            await DeleteKycActivityInfo(v_update_kyc_main_info_result.kycId.Value, kyc.activityInfoList, oracle_conn, transaction);

                            if (kyc.activityInfoList?.Count > 0)
                            {
                                var v_update_kyc_activity_info_result = await UpdateKycActivityInfo(v_update_kyc_main_info_result.kycId.Value, kyc.activityInfoList, oracle_conn, transaction);
                                if (!v_update_kyc_activity_info_result.success)
                                {
                                    return v_update_kyc_activity_info_result;
                                }
                            }


                            await DeleteKycForeignCountryTaxPayerInfo(v_update_kyc_main_info_result.kycId.Value, kyc.foreignCountryTaxPayerInfoList, oracle_conn, transaction);

                            if (kyc.foreignCountryTaxPayerInfoList?.Count > 0)
                            {
                                var v_update_kyc_foreign_country_tax_payer_info_result = await UpdateKycForeignCountryTaxPayerInfo(v_update_kyc_main_info_result.kycId.Value, kyc.foreignCountryTaxPayerInfoList, oracle_conn, transaction);
                                if (!v_update_kyc_foreign_country_tax_payer_info_result.success)
                                {
                                    return v_update_kyc_foreign_country_tax_payer_info_result;
                                }                                
                            }

                            await DeleteKycOtherBankAccount(v_update_kyc_main_info_result.kycId.Value, kyc.otherBankAccountInfoList, oracle_conn, transaction);

                            if (kyc.otherBankAccountInfoList?.Count > 0)
                            {
                                var v_update_kyc_other_bank_account_info_result = await UpdateKycOtherBankAccountInfo(v_update_kyc_main_info_result.kycId.Value, kyc.otherBankAccountInfoList, oracle_conn, transaction);
                                if (!v_update_kyc_other_bank_account_info_result.success)
                                {
                                    return v_update_kyc_other_bank_account_info_result;
                                }
                            }

                            await DeleteKycOtherPerson(v_update_kyc_main_info_result.kycId.Value, kyc.otherPersonInfoList, oracle_conn, transaction);

                            if (kyc.otherPersonInfoList?.Count > 0)
                            {
                                var v_update_kyc_other_person_info_result = await UpdateKycOtherPersonInfo(v_update_kyc_main_info_result.kycId.Value, kyc.otherPersonInfoList, oracle_conn, transaction);
                                if (!v_update_kyc_other_person_info_result.success)
                                {
                                    return v_update_kyc_other_person_info_result;
                                }                                
                            }

                            await DeleteKycProperty(v_update_kyc_main_info_result.kycId.Value, kyc.PropertyInfoList, oracle_conn, transaction);

                            if (kyc.PropertyInfoList?.Count > 0)
                            {
                                var v_update_kyc_property_info_result = await UpdateKycPropertyInfo(v_update_kyc_main_info_result.kycId.Value, kyc.PropertyInfoList, oracle_conn, transaction);
                                if (!v_update_kyc_property_info_result.success)
                                {
                                    return v_update_kyc_property_info_result;
                                }                                
                            }

                            transaction.Commit();
                            v_result = v_update_kyc_main_info_result;
                        }
                        catch (Exception exc)
                        {
                            transaction.Rollback();
                            v_result.success = false;
                            v_result.error = new();
                            v_result.error.errorCode = -1;
                            v_result.error.errorMessageGe = "ზოგადი შეცდომა სერვისის გამოძახებისას";
                            v_result.error.errorMessageEn = String.Format("General error. Error details: {0}", exc.Message + exc.StackTrace);
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                v_result.success = false;
                v_result.error = new();
                v_result.error.errorCode = -1;
                v_result.error.errorMessageGe = "ზოგადი შეცდომა სერვისის გამოძახებისას";
                v_result.error.errorMessageEn = String.Format("General error. Error details: {0}", exc.Message + exc.StackTrace);
            }


            return v_result;
        }




        private async Task<KYCResult> UpdateKycMainInfo(KYC kyc, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();
            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                cmd.Parameters.Add("p_customer_no", OracleDbType.Int32).Value = kyc.customerNo;
                cmd.Parameters.Add("p_monthly_income_code", OracleDbType.Varchar2).Value = kyc.monthlyIncomeCode;
                cmd.Parameters.Add("p_expected_trn_count_code", OracleDbType.Varchar2).Value = kyc.expectedTransactionCountCode;
                cmd.Parameters.Add("p_expected_trn_amount_code", OracleDbType.Varchar2).Value = kyc.expectedTransactionAmountCode;
                cmd.Parameters.Add("p_service_receive_channel_code", OracleDbType.Varchar2).Value = kyc.serviceReceiveChannelCode;
                cmd.Parameters.Add("p_has_inter_trans", OracleDbType.Int16).Value = kyc.hasInternationalTransactions == null ? DBNull.Value : Convert.ToInt16(kyc.hasInternationalTransactions.Value);
                cmd.Parameters.Add("p_inter_tran_countries", OracleDbType.Varchar2).Value = kyc.internationTransactionCountries == null ? "" : String.Join(",", kyc.internationTransactionCountries.Select(x => x));
                cmd.Parameters.Add("p_account_open_reason_in_geo", OracleDbType.Varchar2).Value = kyc.accountOpenReasonInGeorgia;
                cmd.Parameters.Add("p_has_green_card", OracleDbType.Int16).Value = kyc.hasGreenCard == null ? DBNull.Value : Convert.ToInt16(kyc.hasGreenCard.Value);
                cmd.Parameters.Add("p_is_foreign_country_payer", OracleDbType.Int16).Value = kyc.isForeignCountryPayer == null ? DBNull.Value : Convert.ToInt16(kyc.isForeignCountryPayer.Value);
                cmd.Parameters.Add("p_is_pep", OracleDbType.Int16).Value = kyc.isPEP == null ? DBNull.Value : Convert.ToInt16(kyc.isPEP.Value);
                cmd.Parameters.Add("p_is_pep_civil_servant", OracleDbType.Int16).Value = kyc.pepDetails == null ? DBNull.Value : (kyc.pepDetails.isPepCivilServant ==  null ? DBNull.Value : Convert.ToInt16(kyc.pepDetails.isPepCivilServant.Value));
                cmd.Parameters.Add("p_pep_family_member_type_code", OracleDbType.Varchar2).Value = kyc.pepDetails == null ? "" : kyc.pepDetails.pepFamilyMemberTypeCode;
                cmd.Parameters.Add("p_pep_business_relation_code", OracleDbType.Varchar2).Value = kyc.pepDetails == null ? "" : kyc.pepDetails.pepBusinesRelationCode;
                cmd.Parameters.Add("p_related_pep_full_name", OracleDbType.Varchar2).Value = kyc.pepDetails == null ? "" : kyc.pepDetails.relatedPepFullName;
                cmd.Parameters.Add("p_pep_position_code", OracleDbType.Varchar2).Value = kyc.pepDetails == null ? "" : kyc.pepDetails.pepPositionCode;
                cmd.Parameters.Add("p_pep_country", OracleDbType.Varchar2).Value = kyc.pepDetails == null ? "" : kyc.pepDetails.pepCountry;
                cmd.Parameters.Add("p_pep_end_date", OracleDbType.Date).Value = kyc.pepDetails == null ? DBNull.Value : (kyc.pepDetails.pepEndDate == null ? DBNull.Value : Convert.ToDateTime(kyc.pepDetails.pepEndDate.Value));
                cmd.Parameters.Add("p_pep_category_code", OracleDbType.Varchar2).Value = kyc.pepDetails == null ? "" : kyc.pepDetails.pepCategoryCode;
                cmd.Parameters.Add("p_has_account_in_other_banks", OracleDbType.Int16).Value = kyc.hasAccountInOtherBanks == null ? DBNull.Value : Convert.ToInt16(kyc.hasAccountInOtherBanks.Value);
                cmd.Parameters.Add("p_acts_for_other_person", OracleDbType.Int16).Value = kyc.actsForOtherPerson == null ? DBNull.Value : Convert.ToInt16(kyc.actsForOtherPerson.Value);
                cmd.Parameters.Add("p_employment_status_codes", OracleDbType.Varchar2).Value = kyc.employmentStatusCodes == null ? "" : String.Join(",", kyc.employmentStatusCodes.Select(x => x));
                cmd.Parameters.Add("p_income_source_codes", OracleDbType.Varchar2).Value = kyc.incomeSourceCodes == null ? "" : String.Join(",", kyc.incomeSourceCodes.Select(x => x));
                cmd.Parameters.Add("p_bank_relation_purpose_codes", OracleDbType.Varchar2).Value = kyc.bankRelationPurposeCodes == null ? "" : String.Join(",", kyc.bankRelationPurposeCodes.Select(x => x));
                cmd.Parameters.Add("p_bank_relation_product_codes", OracleDbType.Varchar2).Value = kyc.bankRelationProductCodes == null ? "" : String.Join(",", kyc.bankRelationProductCodes.Select(x => x));
                cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                await cmd.ExecuteNonQueryAsync();
                v_result.kycId = Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value);

                var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                if (v_result.kycId == 0)
                {
                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            v_result.success = false;
                            v_result.error = new();
                            v_result.error.errorCode = ora_reader.GetInt32("error_code");
                            v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                            v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                        }
                    }
                }
                else
                {
                    v_result.success = true;
                }

                await ora_reader.CloseAsync();

            }

            return v_result;
        }

        private async Task<KYCResult> UpdateKycEmployerInfo(int kycId, List<EmployerInfo> employerInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();

            foreach (var item in employerInfoList)
            {
                using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC_WORK", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = item.id == null ? DBNull.Value : Convert.ToInt32(item.id.Value);
                    cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                    cmd.Parameters.Add("p_work_type_code", OracleDbType.Varchar2).Value = "EMPLOYER";
                    cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = item.name;
                    cmd.Parameters.Add("p_ident_number", OracleDbType.Varchar2).Value = item.identNumber;
                    cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = item.address;
                    cmd.Parameters.Add("p_web_site", OracleDbType.Varchar2).Value = item.webSite;
                    cmd.Parameters.Add("p_field_of_activity_code", OracleDbType.Varchar2).Value = item.fieldOfActivityCode;
                    cmd.Parameters.Add("p_activity_country", OracleDbType.Varchar2).Value = item.activityCountry;
                    cmd.Parameters.Add("p_position", OracleDbType.Varchar2).Value = item.position;

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            if (!v_result.success)
                            {
                                v_result.error = new();
                                v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                                return v_result;
                            }
                        }
                    }

                    await ora_reader.CloseAsync();
                }
            }

            return v_result;
        }

        private async Task<KYCResult> UpdateKycActivityInfo(int kycId, List<ActivityInfo> activityInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();

            foreach (var item in activityInfoList)
            {
                using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC_WORK", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = item.id == null ? DBNull.Value : Convert.ToInt32(item.id.Value);
                    cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                    cmd.Parameters.Add("p_work_type_code", OracleDbType.Varchar2).Value = "BUSINESS";
                    cmd.Parameters.Add("p_name", OracleDbType.Varchar2).Value = item.name;
                    cmd.Parameters.Add("p_ident_number", OracleDbType.Varchar2).Value = item.identNumber;
                    cmd.Parameters.Add("p_address", OracleDbType.Varchar2).Value = item.address;
                    cmd.Parameters.Add("p_web_site", OracleDbType.Varchar2).Value = item.webSite;
                    cmd.Parameters.Add("p_field_of_activity_code", OracleDbType.Varchar2).Value = item.fieldOfActivityCode;
                    cmd.Parameters.Add("p_activity_country", OracleDbType.Varchar2).Value = item.activityCountry;
                    cmd.Parameters.Add("p_position", OracleDbType.Varchar2).Value = "";

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            if (!v_result.success)
                            {
                                v_result.error = new();
                                v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                                return v_result;
                            }
                        }
                    }

                    await ora_reader.CloseAsync();
                }
            }

            return v_result;
        }

        private async Task<KYCResult> UpdateKycForeignCountryTaxPayerInfo(int kycId, List<ForeignCountryTaxPayerInfo> foreignCountryTaxPayerInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();
            foreach (var item in foreignCountryTaxPayerInfoList)
            {
                using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC_PAYER_FOREIGN_CNTR", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_id", OracleDbType.Varchar2).Value = item.id;
                    cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                    cmd.Parameters.Add("p_country", OracleDbType.Varchar2).Value = item.country;
                    cmd.Parameters.Add("p_tax_number", OracleDbType.Varchar2).Value = item.taxNumber;

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            if (!v_result.success)
                            {
                                v_result.error = new();
                                v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                                return v_result;
                            }
                        }
                    }

                    await ora_reader.CloseAsync();
                }
            }

            return v_result;
        }

        private async Task<KYCResult> UpdateKycOtherBankAccountInfo(int kycId, List<OtherBankAccountInfo> otherBankAccountInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();
            foreach (var item in otherBankAccountInfoList)
            {
                using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC_ACCS_IN_OTHER_BANK", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_id", OracleDbType.Varchar2).Value = item.id;
                    cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                    cmd.Parameters.Add("p_bank_name", OracleDbType.Varchar2).Value = item.bankName;
                    cmd.Parameters.Add("p_country", OracleDbType.Varchar2).Value = item.country;

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            if (!v_result.success)
                            {
                                v_result.error = new();
                                v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                                return v_result;
                            }
                        }
                    }

                    await ora_reader.CloseAsync();
                }
            }

            return v_result;
        }

        private async Task<KYCResult> UpdateKycOtherPersonInfo(int kycId, List<OtherPersonInfo> otherPersonInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();
            foreach (var item in otherPersonInfoList)
            {
                using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC_THIRD_PERSON", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_id", OracleDbType.Varchar2).Value = item.id;
                    cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                    cmd.Parameters.Add("p_person_name", OracleDbType.Varchar2).Value = item.personName;
                    cmd.Parameters.Add("p_ident_number", OracleDbType.Varchar2).Value = item.identNumber;

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            if (!v_result.success)
                            {
                                v_result.error = new();
                                v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                                return v_result;
                            }
                        }
                    }

                    await ora_reader.CloseAsync();
                }
            }

            return v_result;
        }

        private async Task<KYCResult> UpdateKycPropertyInfo(int kycId, List<PropertyInfo> propertyInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            KYCResult v_result = new();
            foreach (var item in propertyInfoList)
            {
                using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.UPDATE_KYC_PROPERTY", connection))
                {
                    cmd.Transaction = transaction;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_id", OracleDbType.Int32).Value = item.id == null ? DBNull.Value : Convert.ToInt32(item.id.Value);
                    cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                    cmd.Parameters.Add("p_property_type_code", OracleDbType.Varchar2).Value = item.propertyTypeCode;
                    cmd.Parameters.Add("p_price", OracleDbType.Decimal).Value = item.price == null ? DBNull.Value : Convert.ToDecimal(item.price.Value);
                    cmd.Parameters.Add("p_currency", OracleDbType.Varchar2).Value = item.currency;
                    cmd.Parameters.Add("p_property_origin_codes", OracleDbType.Varchar2).Value = item.propertyOriginCodes == null ? "" : String.Join(",", item.propertyOriginCodes.Select(x => x));

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    await cmd.ExecuteNonQueryAsync();
                    v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                    if (ora_reader.HasRows)
                    {
                        while (await ora_reader.ReadAsync())
                        {
                            if (!v_result.success)
                            {
                                v_result.error = new();
                                v_result.error.errorCode = ora_reader.GetInt32("error_code");
                                v_result.error.errorMessageGe = await ora_reader.IsDBNullAsync("error_message_ge") ? "" : ora_reader.GetString("error_message_ge");
                                v_result.error.errorMessageEn = await ora_reader.IsDBNullAsync("error_message_en") ? "" : ora_reader.GetString("error_message_en");
                                return v_result;
                            }
                        }
                    }

                    await ora_reader.CloseAsync();
                }
            }

            return v_result;
        }

        private async Task DeleteKycEmployerInfo(int kycId, List<EmployerInfo> employerInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            List<int> v_emp_info_list_ids = null;
            if(employerInfoList != null)
                v_emp_info_list_ids =  employerInfoList.Where(x => x.id != null).Select(x => x.id.Value).ToList();

            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.DELETE_KYC_WORK", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                cmd.Parameters.Add("p_work_type_code", OracleDbType.Varchar2).Value = "EMPLOYER";
                cmd.Parameters.Add("p_ids", OracleDbType.Varchar2).Value = v_emp_info_list_ids == null ? "" : String.Join(",", v_emp_info_list_ids.Select(x => x.ToString()));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task DeleteKycActivityInfo(int kycId, List<ActivityInfo> activityInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            List<int> v_act_info_list_ids = null;
            if(activityInfoList != null)
                v_act_info_list_ids = activityInfoList.Where(x => x.id != null).Select(x => x.id.Value).ToList();

            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.DELETE_KYC_WORK", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                cmd.Parameters.Add("p_work_type_code", OracleDbType.Varchar2).Value = "BUSINESS";
                cmd.Parameters.Add("p_ids", OracleDbType.Varchar2).Value = v_act_info_list_ids == null ? "" : String.Join(",", v_act_info_list_ids.Select(x => x.ToString()));

                await cmd.ExecuteNonQueryAsync();
            }

        }

        private async Task DeleteKycForeignCountryTaxPayerInfo(int kycId, List<ForeignCountryTaxPayerInfo> foreignCountryTaxPayerInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            List<string> v_foreign_country_tax_payer_info_list_ids = null;
            if (foreignCountryTaxPayerInfoList != null)
                v_foreign_country_tax_payer_info_list_ids = foreignCountryTaxPayerInfoList.Where(x => !String.IsNullOrEmpty(x.id)).Select(x => x.id).ToList();

            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.DELETE_KYC_PAYER_FOREIGN_CNTR", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                cmd.Parameters.Add("p_ids", OracleDbType.Varchar2).Value = v_foreign_country_tax_payer_info_list_ids == null ? "" : String.Join(",", v_foreign_country_tax_payer_info_list_ids.Select(x => x.ToString()));

                await cmd.ExecuteNonQueryAsync();
            }

        }

        private async Task DeleteKycOtherBankAccount(int kycId, List<OtherBankAccountInfo> otherBankAccountInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            List<string> v_other_bank_account_ids = null;
            if (otherBankAccountInfoList != null)
                v_other_bank_account_ids = otherBankAccountInfoList.Where(x => !String.IsNullOrEmpty(x.id)).Select(x => x.id).ToList();

            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.DELETE_KYC_ACCS_IN_OTHER_BANK", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                cmd.Parameters.Add("p_ids", OracleDbType.Varchar2).Value = v_other_bank_account_ids == null ? "" : String.Join(",", v_other_bank_account_ids.Select(x => x.ToString()));

                await cmd.ExecuteNonQueryAsync();
            }

        }


        private async Task DeleteKycOtherPerson(int kycId, List<OtherPersonInfo> otherPersonInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            List<string> v_other_person_ids = null;
            if (otherPersonInfoList != null)
                v_other_person_ids = otherPersonInfoList.Where(x => !String.IsNullOrEmpty(x.id)).Select(x => x.id).ToList();

            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.DELETE_KYC_THIRD_PERSON", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                cmd.Parameters.Add("p_ids", OracleDbType.Varchar2).Value = v_other_person_ids == null ? "" : String.Join(",", v_other_person_ids.Select(x => x.ToString()));

                await cmd.ExecuteNonQueryAsync();
            }

        }


        private async Task DeleteKycProperty(int kycId, List<PropertyInfo> propertyInfoList, OracleConnection connection, OracleTransaction transaction)
        {
            List<int> v_property_info_ids = null;
            if (propertyInfoList != null)
                v_property_info_ids = propertyInfoList.Where(x =>x.id != null).Select(x => x.id.Value).ToList();

            using (OracleCommand cmd = new OracleCommand("INTEGRATIONS.DELETE_KYC_THIRD_PERSON", connection))
            {
                cmd.Transaction = transaction;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.BindByName = true;
                if (_commandTimeout != null)
                    cmd.CommandTimeout = _commandTimeout.Value;

                cmd.Parameters.Add("p_kyc_id", OracleDbType.Int32).Value = kycId;
                cmd.Parameters.Add("p_ids", OracleDbType.Varchar2).Value = v_property_info_ids == null ? "" : String.Join(",", v_property_info_ids.Select(x => x.ToString()));

                await cmd.ExecuteNonQueryAsync();
            }

        }



    }

}
