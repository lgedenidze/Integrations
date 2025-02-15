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
    public class CustomersRepository : ICustomersRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public CustomersRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }

        public async Task<CheckCustomerResult> CustomerExists(string personalNumber, string financialNumber)
        {
            CheckCustomerResult v_result = null;
            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.CUSTOMER_EXISTS";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("P_PERSONAL_NO", OracleDbType.Varchar2).Value = personalNumber;
                    cmd.Parameters.Add("P_FIN_MOB", OracleDbType.Varchar2).Value = financialNumber;
                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                    if (ora_reader.HasRows)
                    {
                        v_result = new CheckCustomerResult();
                        while (await ora_reader.ReadAsync())
                        {
                            v_result.customerId = ora_reader.GetInt32("no");
                            v_result.financialNumberMatched = Convert.ToBoolean(ora_reader.GetInt16("fin_num_matched"));
                            v_result.hasValidIdDocument = Convert.ToBoolean(ora_reader.GetInt16("has_valid_id_doc")); 
                            v_result.HasActiveCurrenctAccount = Convert.ToBoolean(ora_reader.GetInt16("has_active_account"));
                            v_result.IsKYCValid = Convert.ToBoolean(ora_reader.GetInt16("is_kyc_valid"));
                        }
                    }
                    await ora_reader.CloseAsync();

                    await oracle_conn.CloseAsync();
                    await oracle_conn.DisposeAsync();
                }
            }

            return v_result;
        }

        public async Task<CreateCustomerResult> CreateCustomer(Customer customer)
        {
            CreateCustomerResult v_result = new();
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.CREATE_CUSTOMER";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_key_cloak_id", OracleDbType.Varchar2).Value = customer.KeyCloakId;
                        cmd.Parameters.Add("p_personal_no", OracleDbType.Varchar2).Value = customer.PersonalNo;
                        cmd.Parameters.Add("p_entrepreneurial_status", OracleDbType.Varchar2).Value = customer.EntrepreneurialStatus == null ? DBNull.Value : customer.EntrepreneurialStatus.ToString();
                        cmd.Parameters.Add("p_first_name_ge", OracleDbType.Varchar2).Value = customer.FirstNameGe;
                        cmd.Parameters.Add("p_last_name_ge", OracleDbType.Varchar2).Value = customer.LastNameGe;
                        cmd.Parameters.Add("p_patronymic_name_ge", OracleDbType.Varchar2).Value = customer.PatronymicNameGe;
                        cmd.Parameters.Add("p_first_name_en", OracleDbType.Varchar2).Value = customer.FirstNameEn;
                        cmd.Parameters.Add("p_last_name_en", OracleDbType.Varchar2).Value = customer.LastNameEn;
                        cmd.Parameters.Add("p_patronymic_name_en", OracleDbType.Varchar2).Value = customer.PatronymicNameEn;
                        cmd.Parameters.Add("p_birth_date", OracleDbType.Date).Value = customer.BirthDate == null ? DBNull.Value : customer.BirthDate.Value;
                        cmd.Parameters.Add("p_birth_country", OracleDbType.Varchar2).Value = customer.BirthCountry;
                        cmd.Parameters.Add("p_birth_city", OracleDbType.Varchar2).Value = customer.BirthCity;
                        cmd.Parameters.Add("p_sex", OracleDbType.Varchar2).Value = customer.Sex == null ? DBNull.Value : customer.Sex.ToString();
                        cmd.Parameters.Add("p_citizenship", OracleDbType.Varchar2).Value = customer.Citizenship;
                        cmd.Parameters.Add("p_double_citizenship", OracleDbType.Varchar2).Value = customer.DoubleCitizenship;
                        cmd.Parameters.Add("p_financial_number", OracleDbType.Varchar2).Value = customer.FinancialNumber;
                        cmd.Parameters.Add("p_additional_mob_number", OracleDbType.Varchar2).Value = customer.AdditionalMobileNumber;
                        cmd.Parameters.Add("p_marketing_sms", OracleDbType.Int16).Value = (customer.MarketingSms == null ? (object)DBNull.Value : Convert.ToInt16(customer.MarketingSms.Value));
                        cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = customer.Email;
                        cmd.Parameters.Add("p_legal_address_country", OracleDbType.Varchar2).Value = customer.LegalAddressCountry;
                        cmd.Parameters.Add("p_legal_address_city_ge", OracleDbType.Varchar2).Value = customer.LegalAddressCityGe;
                        cmd.Parameters.Add("p_legal_address_city_en", OracleDbType.Varchar2).Value = customer.LegalAddressCityEn;
                        cmd.Parameters.Add("p_legal_address_region_id", OracleDbType.Int32).Value = customer.LegalAddressRegionId == null ? DBNull.Value : customer.LegalAddressRegionId;
                        cmd.Parameters.Add("p_legal_address_district_id", OracleDbType.Int32).Value = customer.LegalAddressDistrictId == null ? DBNull.Value : customer.LegalAddressDistrictId; 
                        cmd.Parameters.Add("p_legal_address_ge", OracleDbType.Varchar2).Value = customer.LegalAddressGe;
                        cmd.Parameters.Add("p_legal_address_en", OracleDbType.Varchar2).Value = customer.LegalAddressEn;
                        cmd.Parameters.Add("p_actual_address_country", OracleDbType.Varchar2).Value = customer.ActualAddressCountry;
                        cmd.Parameters.Add("p_actual_address_city_ge", OracleDbType.Varchar2).Value = customer.ActualAddressCityGe;
                        cmd.Parameters.Add("p_actual_address_city_en", OracleDbType.Varchar2).Value = customer.ActualAddressCityEn;
                        cmd.Parameters.Add("p_actual_address_region_id", OracleDbType.Int32).Value = customer.ActualAddressRegionId == null ? DBNull.Value : customer.ActualAddressRegionId; 
                        cmd.Parameters.Add("p_actual_address_district_id", OracleDbType.Int32).Value = customer.ActualAddressDistrictId == null ? DBNull.Value : customer.ActualAddressDistrictId; 
                        cmd.Parameters.Add("p_actual_address_ge", OracleDbType.Varchar2).Value = customer.ActualAddressGe;
                        cmd.Parameters.Add("p_actual_address_en", OracleDbType.Varchar2).Value = customer.ActualAddressEn;
                        cmd.Parameters.Add("p_document_type", OracleDbType.Varchar2).Value = customer.DocumentType == null ? DBNull.Value : customer.DocumentType.ToString();
                        cmd.Parameters.Add("p_document_no", OracleDbType.Varchar2).Value = customer.DocumentNo;
                        cmd.Parameters.Add("p_document_issue_date", OracleDbType.Date).Value = customer.DocumentIssueDate == null ? DBNull.Value : customer.DocumentIssueDate;
                        cmd.Parameters.Add("p_document_expire_date", OracleDbType.Date).Value = customer.DocumentExpireDate == null ? DBNull.Value : customer.DocumentExpireDate;
                        cmd.Parameters.Add("p_document_issuer", OracleDbType.Varchar2).Value = customer.DocumentIssuer;
                        cmd.Parameters.Add("p_document_issuer_country", OracleDbType.Varchar2).Value = customer.DocumentIssuerCountry;
                        cmd.Parameters.Add("p_document_photo_front_base64", OracleDbType.Clob).Value = customer.DocumentPhotoFrontBase64;
                        cmd.Parameters.Add("p_document_photo_back_base64", OracleDbType.Clob).Value = customer.DocumentPhotoBackBase64;
                        cmd.Parameters.Add("p_organization_name", OracleDbType.Varchar2).Value = customer.OrganizationName;
                        cmd.Parameters.Add("p_job_position", OracleDbType.Varchar2).Value = customer.JobPosition;
                        cmd.Parameters.Add("p_field_of_activity_code", OracleDbType.Varchar2).Value = customer.FieldOfActivityCode;
                        cmd.Parameters.Add("p_is_identified_online", OracleDbType.Int16).Value = (customer.IsIdentifiedOnline == null ? (object)DBNull.Value : Convert.ToInt16(customer.IsIdentifiedOnline.Value));
                        cmd.Parameters.Add("p_channel", OracleDbType.Varchar2).Value = customer.Channel;

                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();
                        v_result.CustomerId = Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value);
                        var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                        if (v_result.CustomerId == 0)
                        {
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
                        }

                        await ora_reader.CloseAsync();

                        await oracle_conn.CloseAsync();
                        await oracle_conn.DisposeAsync();
                    }
                }
            }
            catch(Exception exc)
            {
                v_result.CustomerId = 0;
                v_result.error = new();
                v_result.error.errorCode = -1;
                v_result.error.errorMessageGe = "ზოგადი შეცდომა სერვისის გამოძახებისას";
                v_result.error.errorMessageEn = String.Format("General error. Error details: {0}", exc.Message + exc.StackTrace ) ;
            }

            return v_result;
        }

        public async Task<Customer> GetCustomer(int? customerNo, string personalNo)
        {
            Customer v_customer = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region get_customer
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.GET_CUSTOMER";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("P_CUSTOMER_ID", OracleDbType.Int32).Value = customerNo;
                    cmd.Parameters.Add("P_PERSONAL_NO", OracleDbType.Varchar2).Value = personalNo;

                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                    if (ora_reader.HasRows)
                    {
                        v_customer = new Customer();
                        while (await ora_reader.ReadAsync())
                        {
                            v_customer.CustomerId = ora_reader.GetInt32(ora_reader.GetOrdinal("no"));
                            v_customer.KeyCloakId = await ora_reader.IsDBNullAsync("keycloak_id") ? "" : ora_reader.GetString("keycloak_id");
                            v_customer.PersonalNo = await ora_reader.IsDBNullAsync("personal_no") ? "" : ora_reader.GetString("personal_no");
                            v_customer.EntrepreneurialStatus = await ora_reader.IsDBNullAsync("cust_type") ? null : (EntrepreneurialStatus)Enum.Parse(typeof(EntrepreneurialStatus), ora_reader.GetString("cust_type"));
                            v_customer.FirstNameGe = await ora_reader.IsDBNullAsync("first_name_ge") ? "" : ora_reader.GetString("first_name_ge");
                            v_customer.LastNameGe = await ora_reader.IsDBNullAsync("last_name_ge") ? "" : ora_reader.GetString("last_name_ge");
                            v_customer.PatronymicNameGe = await ora_reader.IsDBNullAsync("patronymic_ge") ? "" : ora_reader.GetString("patronymic_ge");
                            v_customer.FirstNameEn = await ora_reader.IsDBNullAsync("first_name_en") ? "" : ora_reader.GetString("first_name_en");
                            v_customer.LastNameEn = await ora_reader.IsDBNullAsync("last_name_en") ? "" : ora_reader.GetString("last_name_en");
                            v_customer.PatronymicNameEn = await ora_reader.IsDBNullAsync("patronymic_en") ? "" : ora_reader.GetString("patronymic_en");
                            v_customer.BirthDate = await ora_reader.IsDBNullAsync("birth_date") ? null :  ora_reader.GetDateTime("birth_date");
                            v_customer.BirthCountry = await ora_reader.IsDBNullAsync("birth_country") ? "" : ora_reader.GetString("birth_country");
                            v_customer.BirthCity = await ora_reader.IsDBNullAsync("birth_city") ? "" : ora_reader.GetString("birth_city");
                            v_customer.Sex = await ora_reader.IsDBNullAsync("sex") ? null : (Sex)Enum.Parse(typeof(Sex), ora_reader.GetString("sex"));
                            v_customer.Citizenship = await ora_reader.IsDBNullAsync("citizenship") ? "" : ora_reader.GetString("citizenship");
                            v_customer.DoubleCitizenship = await ora_reader.IsDBNullAsync("double_citizenship") ? "" : ora_reader.GetString("double_citizenship");
                            v_customer.FinancialNumber = await ora_reader.IsDBNullAsync("financial_number") ? "" : ora_reader.GetString("financial_number");
                            v_customer.AdditionalMobileNumber = await ora_reader.IsDBNullAsync("additional_mobile_number") ? "" : ora_reader.GetString("additional_mobile_number");
                            v_customer.Email = await ora_reader.IsDBNullAsync("email") ? "" : ora_reader.GetString("email");
                            v_customer.LegalAddressCountry = await ora_reader.IsDBNullAsync("legal_address_country") ? "" : ora_reader.GetString("legal_address_country");
                            v_customer.LegalAddressCityGe = await ora_reader.IsDBNullAsync("legal_address_city_ge") ? "" : ora_reader.GetString("legal_address_city_ge");
                            v_customer.LegalAddressCityEn = await ora_reader.IsDBNullAsync("legal_address_city_en") ? "" : ora_reader.GetString("legal_address_city_en");
                            v_customer.LegalAddressRegionId = await ora_reader.IsDBNullAsync("legal_address_region_id") ? null : ora_reader.GetInt32("legal_address_region_id");
                            v_customer.LegalAddressDistrictId = await ora_reader.IsDBNullAsync("legal_address_district_id") ? null : ora_reader.GetInt32("legal_address_district_id");
                            v_customer.LegalAddressGe = await ora_reader.IsDBNullAsync("legal_address_ge") ? "" : ora_reader.GetString("legal_address_ge");
                            v_customer.LegalAddressEn = await ora_reader.IsDBNullAsync("legal_address_en") ? "" : ora_reader.GetString("legal_address_en");
                            v_customer.ActualAddressCountry = await ora_reader.IsDBNullAsync("actual_address_country") ? "" : ora_reader.GetString("actual_address_country");
                            v_customer.ActualAddressCityGe = await ora_reader.IsDBNullAsync("actual_address_city_ge") ? "" : ora_reader.GetString("actual_address_city_ge");
                            v_customer.ActualAddressCityEn = await ora_reader.IsDBNullAsync("actual_address_city_en") ? "" : ora_reader.GetString("actual_address_city_en");
                            v_customer.ActualAddressRegionId = await ora_reader.IsDBNullAsync("actual_address_region_id") ? null : ora_reader.GetInt32("actual_address_region_id");
                            v_customer.ActualAddressDistrictId = await ora_reader.IsDBNullAsync("actual_address_district_id") ? null : ora_reader.GetInt32("actual_address_district_id");
                            v_customer.ActualAddressGe = await ora_reader.IsDBNullAsync("actual_address_ge") ? "" : ora_reader.GetString("actual_address_ge");
                            v_customer.ActualAddressEn = await ora_reader.IsDBNullAsync("actual_address_en") ? "" : ora_reader.GetString("actual_address_en");
                            v_customer.DocumentType = await ora_reader.IsDBNullAsync("documentType") ? null : (DocumentType)Enum.Parse(typeof(DocumentType), ora_reader.GetString("documentType"));
                            v_customer.DocumentNo = await ora_reader.IsDBNullAsync("document_no") ? "" : ora_reader.GetString("document_no");
                            v_customer.DocumentIssueDate = await ora_reader.IsDBNullAsync("document_issue_date") ? null : ora_reader.GetDateTime("document_issue_date");
                            v_customer.DocumentExpireDate = await ora_reader.IsDBNullAsync("document_expire_date") ? null : ora_reader.GetDateTime("document_expire_date");
                            v_customer.DocumentIssuer = await ora_reader.IsDBNullAsync("document_issuer") ? "" : ora_reader.GetString("document_issuer");
                            v_customer.DocumentIssuerCountry = await ora_reader.IsDBNullAsync("document_issuer_country") ? "" : ora_reader.GetString("document_issuer_country");
                            v_customer.OrganizationName = await ora_reader.IsDBNullAsync("organization_name") ? "" : ora_reader.GetString("organization_name");
                            v_customer.JobPosition = await ora_reader.IsDBNullAsync("job_position") ? "" : ora_reader.GetString("job_position");
                            v_customer.FieldOfActivityCode = await ora_reader.IsDBNullAsync("field_of_activity_code") ? "" : ora_reader.GetString("field_of_activity_code");
                            v_customer.isPep = await ora_reader.IsDBNullAsync("IS_PEP")? null : Convert.ToBoolean(ora_reader.GetInt16("IS_PEP"));
                            v_customer.isResident = await ora_reader.IsDBNullAsync("is_resident")? null : Convert.ToBoolean(ora_reader.GetInt16("is_resident"));

                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
            }


            return v_customer;
        }

        public async Task<UpdateCustomerResult> UpdateCustomer(Customer customer)
        {
            UpdateCustomerResult v_result = new();
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.UPDATE_CUSTOMER";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_customer_no", OracleDbType.Int32).Value = customer.CustomerId;

                        cmd.Parameters.Add("p_personal_no", OracleDbType.Varchar2).Value = customer.PersonalNo;
                        cmd.Parameters.Add("p_entrepreneurial_status", OracleDbType.Varchar2).Value = customer.EntrepreneurialStatus == null ? DBNull.Value : customer.EntrepreneurialStatus.ToString();
                        cmd.Parameters.Add("p_key_cloak_id", OracleDbType.Varchar2).Value = customer.KeyCloakId;
                        cmd.Parameters.Add("p_first_name_ge", OracleDbType.Varchar2).Value = customer.FirstNameGe;
                        cmd.Parameters.Add("p_last_name_ge", OracleDbType.Varchar2).Value = customer.LastNameGe;
                        cmd.Parameters.Add("p_patronymic_name_ge", OracleDbType.Varchar2).Value = customer.PatronymicNameGe;
                        cmd.Parameters.Add("p_first_name_en", OracleDbType.Varchar2).Value = customer.FirstNameEn;
                        cmd.Parameters.Add("p_last_name_en", OracleDbType.Varchar2).Value = customer.LastNameEn;
                        cmd.Parameters.Add("p_patronymic_name_en", OracleDbType.Varchar2).Value = customer.PatronymicNameEn;
                        cmd.Parameters.Add("p_birth_date", OracleDbType.Date).Value = customer.BirthDate == null ? DBNull.Value : customer.BirthDate.Value;
                        cmd.Parameters.Add("p_birth_country", OracleDbType.Varchar2).Value = customer.BirthCountry;
                        cmd.Parameters.Add("p_birth_city", OracleDbType.Varchar2).Value = customer.BirthCity;
                        cmd.Parameters.Add("p_sex", OracleDbType.Varchar2).Value = customer.Sex == null ? DBNull.Value : customer.Sex.ToString();
                        cmd.Parameters.Add("p_citizenship", OracleDbType.Varchar2).Value = customer.Citizenship;
                        cmd.Parameters.Add("p_double_citizenship", OracleDbType.Varchar2).Value = customer.DoubleCitizenship;
                        cmd.Parameters.Add("p_financial_number", OracleDbType.Varchar2).Value = customer.FinancialNumber;
                        cmd.Parameters.Add("p_additional_mob_number", OracleDbType.Varchar2).Value = customer.AdditionalMobileNumber;
                        cmd.Parameters.Add("p_marketing_sms", OracleDbType.Int16).Value = (customer.MarketingSms == null ? (object)DBNull.Value : Convert.ToInt16(customer.MarketingSms.Value));
                        cmd.Parameters.Add("p_email", OracleDbType.Varchar2).Value = customer.Email;
                        cmd.Parameters.Add("p_legal_address_country", OracleDbType.Varchar2).Value = customer.LegalAddressCountry;
                        cmd.Parameters.Add("p_legal_address_city_ge", OracleDbType.Varchar2).Value = customer.LegalAddressCityGe;
                        cmd.Parameters.Add("p_legal_address_city_en", OracleDbType.Varchar2).Value = customer.LegalAddressCityEn;
                        cmd.Parameters.Add("p_legal_address_region_id", OracleDbType.Int32).Value = customer.LegalAddressRegionId == null ? DBNull.Value : customer.LegalAddressRegionId;
                        cmd.Parameters.Add("p_legal_address_district_id", OracleDbType.Int32).Value = customer.LegalAddressDistrictId == null ? DBNull.Value : customer.LegalAddressDistrictId;
                        cmd.Parameters.Add("p_legal_address_ge", OracleDbType.Varchar2).Value = customer.LegalAddressGe;
                        cmd.Parameters.Add("p_legal_address_en", OracleDbType.Varchar2).Value = customer.LegalAddressEn;
                        cmd.Parameters.Add("p_actual_address_country", OracleDbType.Varchar2).Value = customer.ActualAddressCountry;
                        cmd.Parameters.Add("p_actual_address_city_ge", OracleDbType.Varchar2).Value = customer.ActualAddressCityGe;
                        cmd.Parameters.Add("p_actual_address_city_en", OracleDbType.Varchar2).Value = customer.ActualAddressCityEn;
                        cmd.Parameters.Add("p_actual_address_region_id", OracleDbType.Int32).Value = customer.ActualAddressRegionId == null ? DBNull.Value : customer.ActualAddressRegionId;
                        cmd.Parameters.Add("p_actual_address_district_id", OracleDbType.Int32).Value = customer.ActualAddressDistrictId == null ? DBNull.Value : customer.ActualAddressDistrictId;
                        cmd.Parameters.Add("p_actual_address_ge", OracleDbType.Varchar2).Value = customer.ActualAddressGe;
                        cmd.Parameters.Add("p_actual_address_en", OracleDbType.Varchar2).Value = customer.ActualAddressEn;
                        cmd.Parameters.Add("p_document_type", OracleDbType.Varchar2).Value = customer.DocumentType == null ? DBNull.Value : customer.DocumentType.ToString();
                        cmd.Parameters.Add("p_document_no", OracleDbType.Varchar2).Value = customer.DocumentNo;
                        cmd.Parameters.Add("p_document_issue_date", OracleDbType.Date).Value = customer.DocumentIssueDate == null ? DBNull.Value : customer.DocumentIssueDate;
                        cmd.Parameters.Add("p_document_expire_date", OracleDbType.Date).Value = customer.DocumentExpireDate == null ? DBNull.Value : customer.DocumentExpireDate;
                        cmd.Parameters.Add("p_document_issuer", OracleDbType.Varchar2).Value = customer.DocumentIssuer;
                        cmd.Parameters.Add("p_document_issuer_country", OracleDbType.Varchar2).Value = customer.DocumentIssuerCountry;
                        cmd.Parameters.Add("p_document_photo_front_base64", OracleDbType.Clob).Value = customer.DocumentPhotoFrontBase64;
                        cmd.Parameters.Add("p_document_photo_back_base64", OracleDbType.Clob).Value = customer.DocumentPhotoBackBase64;
                        cmd.Parameters.Add("p_organization_name", OracleDbType.Varchar2).Value = customer.OrganizationName;
                        cmd.Parameters.Add("p_job_position", OracleDbType.Varchar2).Value = customer.JobPosition;
                        cmd.Parameters.Add("p_field_of_activity_code", OracleDbType.Varchar2).Value = customer.FieldOfActivityCode;

                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        await cmd.ExecuteNonQueryAsync();
                        v_result.success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                        if (!v_result.success)
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

    }
}
