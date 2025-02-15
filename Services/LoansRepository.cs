using Integrations.Model;
using Microsoft.AspNetCore.Mvc;
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
    public class LoansRepository : ILoansRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public LoansRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }

        public async Task<LoanRegistrationResult> RegisterLoan(LoanRegistrationData loanRegistrationData)
        {
            LoanRegistrationResult v_result = null;


            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region registerLoan
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {

                    cmd.CommandText = "LOAN.create_loan";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_cust_no", OracleDbType.Int32).Value = loanRegistrationData.CustomerNo;
                    cmd.Parameters.Add("p_agreement_id", OracleDbType.Varchar2).Value = loanRegistrationData.AgreementId;
                    cmd.Parameters.Add("p_loan_type", OracleDbType.Varchar2).Value = loanRegistrationData.LoanType;
                    cmd.Parameters.Add("p_loan_amount", OracleDbType.Decimal).Value = loanRegistrationData.Amount;
                    cmd.Parameters.Add("p_loan_ccy", OracleDbType.Varchar2).Value = loanRegistrationData.Currency;
                    cmd.Parameters.Add("p_loan_term", OracleDbType.Int32).Value = loanRegistrationData.Term;
                    cmd.Parameters.Add("p_start_date", OracleDbType.Date).Value = loanRegistrationData.StartDate;
                    cmd.Parameters.Add("p_pay_day", OracleDbType.Int32).Value = loanRegistrationData.PayDay;
                    cmd.Parameters.Add("p_periodicity", OracleDbType.Varchar2).Value = loanRegistrationData.PayPeriodicity.ToString();
                    cmd.Parameters.Add("p_loan_kind", OracleDbType.Varchar2).Value = loanRegistrationData.Kind.ToString();
                    cmd.Parameters.Add("p_principal_grace_p", OracleDbType.Int32).Value = (loanRegistrationData.GracePeriodOnPrincipal == null ? (object)DBNull.Value : loanRegistrationData.GracePeriodOnPrincipal.Value);
                    cmd.Parameters.Add("p_interest_grace_p", OracleDbType.Int32).Value = (loanRegistrationData.GracePeriodOnInterest == null ? (object)DBNull.Value : loanRegistrationData.GracePeriodOnInterest.Value);
                    cmd.Parameters.Add("p_principal_grace_p_type", OracleDbType.Varchar2).Value = (loanRegistrationData.GracePeriodOnPrincipalType == null ? "" : loanRegistrationData.GracePeriodOnPrincipalType.ToString());
                    cmd.Parameters.Add("p_interest_grace_p_type", OracleDbType.Varchar2).Value = (loanRegistrationData.GracePeriodOnInterestType == null ? "" : loanRegistrationData.GracePeriodOnInterestType.ToString());
                    cmd.Parameters.Add("p_interest_rate", OracleDbType.Decimal).Value = (loanRegistrationData.InterestRate == null ? (object)DBNull.Value : loanRegistrationData.InterestRate.Value);
                    cmd.Parameters.Add("p_give_out_fee", OracleDbType.Decimal).Value = (loanRegistrationData.GiveOutFee == null ? (object)DBNull.Value : loanRegistrationData.GiveOutFee.ToString());
                    cmd.Parameters.Add("p_give_out_fee_type", OracleDbType.Varchar2).Value = (loanRegistrationData.GiveOutFeeType == null ? "" : loanRegistrationData.GiveOutFeeType.ToString());
                    cmd.Parameters.Add("p_early_repayment_fee", OracleDbType.Decimal).Value = (loanRegistrationData.EarlyRepaymentFee == null ? (object)DBNull.Value : loanRegistrationData.EarlyRepaymentFee.Value);
                    cmd.Parameters.Add("p_effective_rate", OracleDbType.Decimal).Value = (loanRegistrationData.EffectiveRate == null ? (object)DBNull.Value : loanRegistrationData.EffectiveRate.Value);
                    cmd.Parameters.Add("p_effective_rate_fc", OracleDbType.Decimal).Value = (loanRegistrationData.EffectiveRateFC == null ? (object)DBNull.Value : loanRegistrationData.EffectiveRateFC.Value);
                    cmd.Parameters.Add("p_field_of_activity", OracleDbType.Varchar2).Value = loanRegistrationData.FieldOfActivity;
                    cmd.Parameters.Add("p_has_outside_active_loans", OracleDbType.Int16).Value = (loanRegistrationData.HasActiveLoansInOtherBanks == null ? (object)DBNull.Value : Convert.ToInt16(loanRegistrationData.HasActiveLoansInOtherBanks.Value));
                    cmd.Parameters.Add("p_pti", OracleDbType.Decimal).Value = (loanRegistrationData.PTI == null ? (object)DBNull.Value : loanRegistrationData.PTI.Value);
                    cmd.Parameters.Add("p_pti_extended", OracleDbType.Decimal).Value = (loanRegistrationData.PTIExtended == null ? (object)DBNull.Value : loanRegistrationData.PTIExtended.Value);
                    cmd.Parameters.Add("p_is_hedged", OracleDbType.Int16).Value = (loanRegistrationData.IsHedged == null ? (object)DBNull.Value : Convert.ToInt16(loanRegistrationData.IsHedged.Value));
                    cmd.Parameters.Add("p_is_refinancing", OracleDbType.Int16).Value = (loanRegistrationData.IsRefinancing == null ? (object)DBNull.Value : Convert.ToInt16(loanRegistrationData.IsRefinancing.Value));
                    cmd.Parameters.Add("p_is_refinancing_no_cash", OracleDbType.Int16).Value = (loanRegistrationData.IsRefinancingNoCash == null ? (object)DBNull.Value : Convert.ToInt16(loanRegistrationData.IsRefinancingNoCash.Value));
                    cmd.Parameters.Add("p_ref_interest_and_other_am", OracleDbType.Decimal).Value = (loanRegistrationData.RefinancingInterestAndOtherAmount == null ? (object)DBNull.Value : loanRegistrationData.RefinancingInterestAndOtherAmount.Value);
                    cmd.Parameters.Add("p_refinancing_principal_amount", OracleDbType.Decimal).Value = (loanRegistrationData.RefinancingPrincipalAmount == null ? (object)DBNull.Value : loanRegistrationData.RefinancingPrincipalAmount.Value);
                    cmd.Parameters.Add("p_refinanced_loans_bank_codes", OracleDbType.Varchar2).Value = (loanRegistrationData.RefinancedLoans == null ? "" : String.Join(",", loanRegistrationData.RefinancedLoans.Select(x => x.BankCode)));
                    cmd.Parameters.Add("p_loan_approve_date", OracleDbType.Date).Value = loanRegistrationData.LoanAproveDate;
                    cmd.Parameters.Add("p_officer_id", OracleDbType.Varchar2).Value = loanRegistrationData.OfficerID;
                    cmd.Parameters.Add("p_insurance_amt", OracleDbType.Decimal).Value = (loanRegistrationData.InsuranceAmount == null ? (object)DBNull.Value : loanRegistrationData.InsuranceAmount.Value);
                    cmd.Parameters.Add("p_monthly_fee_amt", OracleDbType.Decimal).Value = (loanRegistrationData.MonthlyFeeAmount == null ? (object)DBNull.Value : loanRegistrationData.MonthlyFeeAmount.Value);
                    cmd.Parameters.Add("p_channel", OracleDbType.Varchar2).Value = loanRegistrationData.Channel;
                    cmd.Parameters.Add("p_subscriber_no", OracleDbType.Varchar2).Value = loanRegistrationData.SubscriberNo;
                    cmd.Parameters.Add("p_minimal_pay_amount", OracleDbType.Decimal).Value = (loanRegistrationData.MinimalPayAmount == null ? (object)DBNull.Value : loanRegistrationData.MinimalPayAmount.Value);
                    cmd.Parameters.Add("p_income_calc_source", OracleDbType.Varchar2).Value = loanRegistrationData.IncomeCalculationSource;
                    cmd.Parameters.Add("p_total_liab_gel", OracleDbType.Decimal).Value =  (loanRegistrationData.TotalLiabGel == null ? (object)DBNull.Value : loanRegistrationData.TotalLiabGel.Value);

                    cmd.Parameters.Add("p_total_liab_usd", OracleDbType.Decimal).Value =(loanRegistrationData.TotalLiabUsd == null ? (object)DBNull.Value : loanRegistrationData.TotalLiabUsd.Value);
                    cmd.Parameters.Add("p_total_liab_eur", OracleDbType.Decimal).Value =(loanRegistrationData.TotalLiabEur == null ? (object)DBNull.Value : loanRegistrationData.TotalLiabEur.Value);
                    cmd.Parameters.Add("p_ext_bal_liab_gel", OracleDbType.Decimal).Value =(loanRegistrationData.ExtBalLiabGel == null ? (object)DBNull.Value : loanRegistrationData.ExtBalLiabGel.Value);
                    cmd.Parameters.Add("p_ext_bal_liab_usd", OracleDbType.Decimal).Value =(loanRegistrationData.ExtBalLiabUsd == null ? (object)DBNull.Value : loanRegistrationData.ExtBalLiabUsd.Value);
                    cmd.Parameters.Add("p_ext_bal_liab_eur", OracleDbType.Decimal).Value =(loanRegistrationData.ExtBalLiabEur == null ? (object)DBNull.Value : loanRegistrationData.ExtBalLiabEur.Value);
                    cmd.Parameters.Add("p_risk_grade", OracleDbType.Varchar2).Value =(loanRegistrationData.RiskGrade == null ? (object)DBNull.Value : loanRegistrationData.RiskGrade);

                    cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;


                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    await cmd.ExecuteNonQueryAsync();
                    int returnValue = Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value);

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();
                    if (ora_reader.HasRows)
                    {
                        v_result = new();
                        while (await ora_reader.ReadAsync())
                        {
                            v_result.LoanID = await ora_reader.IsDBNullAsync("loanID") ? null : ora_reader.GetInt32("loanID");
                            v_result.ResultCode = ora_reader.GetInt32("result");
                            v_result.Description = ora_reader.GetString("description");
                        }
                    }

                    await ora_reader.CloseAsync();

                    #endregion
                    if (returnValue > 0)
                    {
                        #region insertRefinincedLoanData
                        if (loanRegistrationData.RefinancedLoans != null)
                        {
                            foreach (var l_ref_loans in loanRegistrationData.RefinancedLoans)
                            {
                                using (OracleCommand cmd_ref = oracle_conn.CreateCommand())
                                {
                                    cmd_ref.CommandText = "LOAN.ADD_REFINANCED_LOAN";
                                    cmd_ref.CommandType = CommandType.StoredProcedure;
                                    cmd_ref.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                                    cmd_ref.Parameters.Add("p_agr_no", OracleDbType.Int32).Value = returnValue;
                                    cmd_ref.Parameters.Add("p_bank_code", OracleDbType.Varchar2).Value = l_ref_loans.BankCode;
                                    cmd_ref.Parameters.Add("p_ref_sb_agr_id", OracleDbType.Varchar2).Value = l_ref_loans.AgreementNoOfSilkBank;
                                    cmd_ref.Parameters.Add("p_ref_sb_agr_no", OracleDbType.Int32).Value = (l_ref_loans.LoanIDOfSilkBank == null ? (object)DBNull.Value : l_ref_loans.LoanIDOfSilkBank.Value);
                                    cmd_ref.Parameters.Add("p_ref_loan_amount", OracleDbType.Decimal).Value = (l_ref_loans.RefinancedLoanAmount == null ? (object)DBNull.Value : l_ref_loans.RefinancedLoanAmount.Value);
                                    cmd_ref.Parameters.Add("p_ref_loan_remainder", OracleDbType.Decimal).Value = (l_ref_loans.RefinancedLoanRemainder == null ? (object)DBNull.Value : l_ref_loans.RefinancedLoanRemainder.Value);
                                    cmd_ref.Parameters.Add("p_ref_id_credit_buereu", OracleDbType.Varchar2).Value = l_ref_loans.RefinancedIDFromCreditBureau;
                                    cmd_ref.Parameters.Add("p_ref_loan_ccy", OracleDbType.Varchar2).Value = l_ref_loans.Currency;
                                    cmd_ref.Parameters.Add("p_ref_loan_type", OracleDbType.Varchar2).Value = l_ref_loans.RefinancedLoanType;

                                    if (oracle_conn.State != ConnectionState.Open)
                                    {
                                        await oracle_conn.OpenAsync();
                                    }

                                    await cmd_ref.ExecuteNonQueryAsync();
                                }
                            }

                        }
                    }

                    #endregion

                    await oracle_conn.CloseAsync();
                    await oracle_conn.DisposeAsync();

                }
            }

            return v_result;
        }


        public async Task<CloseLoanResult> CloseLoan(int loanID)
        {
            CloseLoanResult v_result = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region closeLoan
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "LOAN.close_unsigned_loan";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_agr_no", OracleDbType.Int32).Value = loanID;
                    cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    await cmd.ExecuteNonQueryAsync();
                    int returnValue = Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value);

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();
                    if (ora_reader.HasRows)
                    {
                        v_result = new();
                        while (await ora_reader.ReadAsync())
                        {
                            v_result.ResultCode = ora_reader.GetInt32("result");
                            v_result.Description = ora_reader.GetString("description");
                        }
                    }

                    await ora_reader.CloseAsync();

                    await oracle_conn.CloseAsync();
                    await oracle_conn.DisposeAsync();

                }
                #endregion
            }

            return v_result;
        }

        public async Task<GiveOutLoanResult> GiveOutLoan(GiveOutLoanParameters giveOutLoanParameters)
        {
            GiveOutLoanResult v_result = null;

            using (OracleConnection oracleConnection = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = oracleConnection.CreateCommand())
                {
                    cmd.CommandText = "LOAN.give_out_loan";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("agr_no", OracleDbType.Int32).Value = giveOutLoanParameters.loanID;
                    cmd.Parameters.Add("account_id", OracleDbType.Varchar2).Value = String.IsNullOrEmpty(giveOutLoanParameters.accountID) ? DBNull.Value : giveOutLoanParameters.accountID;
                    cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracleConnection.State != ConnectionState.Open)
                    {
                        await oracleConnection.OpenAsync();
                    }

                    await cmd.ExecuteNonQueryAsync();

                    var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();
                    if (ora_reader.HasRows)
                    {
                        v_result = new();
                        while (await ora_reader.ReadAsync())
                        {
                            v_result.transactionResultCode = await ora_reader.IsDBNullAsync("trnResultCode") ? null : ora_reader.GetInt32("trnResultCode");
                            v_result.transactionResultMessage = await ora_reader.IsDBNullAsync("trnResultCode") ? "" : ora_reader.GetString("trnResultCode");
                            v_result.loanResultCode = await ora_reader.IsDBNullAsync("loanResultCode") ? null : ora_reader.GetInt32("loanResultCode");
                            v_result.loanResultMessage = await ora_reader.IsDBNullAsync("loanResultMessage") ? "" : ora_reader.GetString("loanResultMessage");
                        }
                    }

                    await ora_reader.CloseAsync();

                    await oracleConnection.CloseAsync();
                    await oracleConnection.DisposeAsync();



                }

            }
            return v_result;
        }
        public async Task<GetLoanDetailsResult> GetLoanDetails(int loanID)
        {
            GetLoanDetailsResult getLoanDetailsResult = null;

            using (OracleConnection oracleConn = new OracleConnection(connectionString))
            {
                #region GetLoanDetails
                using (OracleCommand cmd = oracleConn.CreateCommand())
                {
                    cmd.CommandText = "integrations.get_loan_details";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;

                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    // Adding parameters to the command
                    cmd.Parameters.Add("p_agr_no", OracleDbType.Int32).Value = loanID;
                    cmd.Parameters.Add("p_recordset", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                    OracleParameter p_add_info_param = cmd.Parameters.Add("p_add_info", OracleDbType.Clob);
                    p_add_info_param.Direction = ParameterDirection.Output;

                    // Open connection
                    if (oracleConn.State != ConnectionState.Open)
                    {
                        await oracleConn.OpenAsync();
                    }

                    using (DbDataReader oraReader = await cmd.ExecuteReaderAsync())
                    {
                        if (oraReader.HasRows)
                        {
                            getLoanDetailsResult = new GetLoanDetailsResult();

                            while (await oraReader.ReadAsync())
                            {
                                // Assuming your PL/SQL cursor returns a single row, mapping the data
                                getLoanDetailsResult.MonthlyPayment = await oraReader.IsDBNullAsync("monthly_payment") ? null : oraReader.GetDecimal("monthly_payment");
                                getLoanDetailsResult.LastPayDate = await oraReader.IsDBNullAsync("last_pay_date") ? null : oraReader.GetDateTime("last_pay_date");
                                getLoanDetailsResult.NextPayDate = await oraReader.IsDBNullAsync("next_pay_date") ? null : oraReader.GetDateTime("next_pay_date");
                                getLoanDetailsResult.NextPayAmount = await oraReader.IsDBNullAsync("next_pay_amount") ? null : oraReader.GetDecimal("next_pay_amount");
                                getLoanDetailsResult.EffectiveRate = await oraReader.IsDBNullAsync("effective_rate") ? null : oraReader.GetDecimal("effective_rate");
                                getLoanDetailsResult.PayAmountEndOfTerm = await oraReader.IsDBNullAsync("pay_amount_end_of_term") ? null : oraReader.GetDecimal("pay_amount_end_of_term");
                                getLoanDetailsResult.TotalPayment = await oraReader.IsDBNullAsync("total_payment") ? null : oraReader.GetDecimal("total_payment");
                                getLoanDetailsResult.ReceiveAmount = await oraReader.IsDBNullAsync("recive_amount") ? null : oraReader.GetDecimal("recive_amount");
                                getLoanDetailsResult.GiveOutFee = await oraReader.IsDBNullAsync("GIVE_OUT_INT") ? null : oraReader.GetDecimal("GIVE_OUT_INT");
                                getLoanDetailsResult.AdvancePayInt = await oraReader.IsDBNullAsync("ADVANCE_PAY_INT") ? null : oraReader.GetDecimal("ADVANCE_PAY_INT");
                                getLoanDetailsResult.OverduePayInt = await oraReader.IsDBNullAsync("OVERDUE_DAY_PROC") ? null : oraReader.GetDecimal("OVERDUE_DAY_PROC");
                                // getLoanDetailsResult.AdditionalInfo = await oraReader.IsDBNullAsync("ADD_INFO") ? null : oraReader.GetString("ADD_INFO");
                                getLoanDetailsResult.CustomerCommission = await oraReader.IsDBNullAsync("CUST_COM") ? null : oraReader.GetDecimal("CUST_COM");
                                getLoanDetailsResult.FullName = await oraReader.IsDBNullAsync("full_name") ? null : oraReader.GetString("full_name");
                                getLoanDetailsResult.ContactPersonFullName = await oraReader.IsDBNullAsync("contract_person_full_name") ? null : oraReader.GetString("contract_person_full_name");
                                getLoanDetailsResult.LegalAddress = await oraReader.IsDBNullAsync("legal_address") ? null : oraReader.GetString("legal_address");
                                getLoanDetailsResult.ActualAddress = await oraReader.IsDBNullAsync("actial_address") ? null : oraReader.GetString("actial_address");
                                getLoanDetailsResult.PersonalNum = await oraReader.IsDBNullAsync("tax_number") ? null : oraReader.GetString("tax_number");
                                getLoanDetailsResult.MobileNumber = await oraReader.IsDBNullAsync("phone_number") ? null : oraReader.GetString("phone_number");
                                getLoanDetailsResult.AgreementId = await oraReader.IsDBNullAsync("agr_id") ? null : oraReader.GetString("agr_id");
                                getLoanDetailsResult.LoanPurpose = await oraReader.IsDBNullAsync("loan_purpose") ? null : oraReader.GetString("loan_purpose");
                                getLoanDetailsResult.AnnualInterestRate = await oraReader.IsDBNullAsync("annual_int_rate") ? null : oraReader.GetDecimal("annual_int_rate");
                                getLoanDetailsResult.LoanAmount = await oraReader.IsDBNullAsync("loan_amount") ? null : oraReader.GetDecimal("loan_amount");
                                getLoanDetailsResult.FirstPaymentDate = await oraReader.IsDBNullAsync("first_pay_date") ? null : oraReader.GetDateTime("first_pay_date");
                                getLoanDetailsResult.FirstPaymentAmount = await oraReader.IsDBNullAsync("first_paY_amount") ? null : oraReader.GetDecimal("first_paY_amount");
                                getLoanDetailsResult.Currency = await oraReader.IsDBNullAsync("ccy") ? null : oraReader.GetString("ccy");
                                getLoanDetailsResult.LoanRemainMonths = await oraReader.IsDBNullAsync("loan_remain_months") ? null : oraReader.GetInt32("loan_remain_months");

                                getLoanDetailsResult.LimitAmount = await oraReader.IsDBNullAsync("limit") ? null : oraReader.GetDecimal("limit");
                                getLoanDetailsResult.UsedCredit = await oraReader.IsDBNullAsync("used_limit") ? null : oraReader.GetDecimal("used_limit");
                                getLoanDetailsResult.BillingDay = await oraReader.IsDBNullAsync("billing_day") ? null : oraReader.GetInt32("billing_day");
                                getLoanDetailsResult.PaymentDay = await oraReader.IsDBNullAsync("paymenT_day") ? null : oraReader.GetInt32("paymenT_day");
                                getLoanDetailsResult.RecommendedPaymentAmount = await oraReader.IsDBNullAsync("recommended_Payment_Amount") ? null : oraReader.GetDecimal("recommended_Payment_Amount");
                                getLoanDetailsResult.MinimumPaymentAmount = await oraReader.IsDBNullAsync("minimum_Payment_Amount") ? null : oraReader.GetDecimal("minimum_Payment_Amount");
                                getLoanDetailsResult.LimitIssueDate = await oraReader.IsDBNullAsync("limit_issue_date") ? null : oraReader.GetDateTime("limit_issue_date");
                                getLoanDetailsResult.LimitExpireDate = await oraReader.IsDBNullAsync("limit_Exp_Date") ? null : oraReader.GetDateTime("limit_Exp_Date");
                                getLoanDetailsResult.IdentDocumentId = await oraReader.IsDBNullAsync("document_id") ? null : oraReader.GetString("document_id");
                                getLoanDetailsResult.ClientEmail = await oraReader.IsDBNullAsync("email") ? null : oraReader.GetString("email");
                                getLoanDetailsResult.ClientPayIfAtm = await oraReader.IsDBNullAsync("client_pay_if_atm") ? null : oraReader.GetDecimal("client_pay_if_atm");
                                getLoanDetailsResult.ClientPayIfMin = await oraReader.IsDBNullAsync("client_pay_if_min") ? null : oraReader.GetDecimal("client_pay_if_min");
                                getLoanDetailsResult.ServiceAccount = await oraReader.IsDBNullAsync("service_account") ? null : oraReader.GetString("service_account");
                                getLoanDetailsResult.EffectiveNonCash = await oraReader.IsDBNullAsync("effective_non_cash") ? null : oraReader.GetDecimal("effective_non_cash");
                                getLoanDetailsResult.EffectiveWithdrawal = await oraReader.IsDBNullAsync("effective_withdrawal") ? null : oraReader.GetDecimal("effective_withdrawal");
                                getLoanDetailsResult.EffectiveRecommended = await oraReader.IsDBNullAsync("effecitve_recommended") ? null : oraReader.GetDecimal("effecitve_recommended");
                                getLoanDetailsResult.EffectiveProlong = await oraReader.IsDBNullAsync("effecitve_prolong") ? null : oraReader.GetDecimal("effecitve_prolong");
                                getLoanDetailsResult.LoanTerm = await oraReader.IsDBNullAsync("loan_term") ? null : oraReader.GetInt32("loan_term");
                                getLoanDetailsResult.MinimumPaymentAmountFixed= await oraReader.IsDBNullAsync("fixed_min_amount") ? null : oraReader.GetDecimal("fixed_min_amount");

                            }
                            await oraReader.CloseAsync();

                            getLoanDetailsResult.AdditionalInfo = ((OracleClob)p_add_info_param.Value).Value;
                        }
                    }
                    await oracleConn.CloseAsync();
                    await oracleConn.DisposeAsync();
                }

                return getLoanDetailsResult;
                #endregion
            }
        }

        public async Task<GeneralResult> LoanRepayment(LoanRepaymentParams loanRepaymentParams)
        {
            GeneralResult result = null;
            #region LoanRepayment
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {

                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.LOAN_REPAYMENT";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        // Set up parameters
                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("P_AGR_NO", OracleDbType.Int32).Value = loanRepaymentParams.loanID;
                        cmd.Parameters.Add("P_ACC_ID", OracleDbType.Varchar2).Value = loanRepaymentParams.accountID;
                        cmd.Parameters.Add("P_AMOUNT", OracleDbType.Decimal).Value = loanRepaymentParams.amount;
                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }

                        // Execute command asynchronously
                        await cmd.ExecuteNonQueryAsync();

                        bool v_success = Convert.ToBoolean(Convert.ToInt32((int)(OracleDecimal)cmd.Parameters["return_value"].Value));

                        // Process results asynchronously
                        var refCursorParam = (OracleRefCursor)cmd.Parameters["P_RECORDSET"].Value;
                        result = new();
                        result.success = v_success;

                        if (!v_success)
                        {
                            var ora_reader = ((OracleRefCursor)cmd.Parameters["p_recordset"].Value).GetDataReader();

                            if (ora_reader.HasRows)
                                while (await ora_reader.ReadAsync()) // Asynchronously read each row
                                {
                                    if (result.error == null)
                                    {
                                        result.error = new Error();
                                    }

                                    result.error.errorCode = ora_reader.GetInt32(ora_reader.GetOrdinal("error_code"));
                                    result.error.errorMessageGe = ora_reader.IsDBNull(ora_reader.GetOrdinal("error_message_ge")) ? "" : ora_reader.GetString(ora_reader.GetOrdinal("error_message_ge"));
                                    result.error.errorMessageEn = ora_reader.IsDBNull(ora_reader.GetOrdinal("error_message_en")) ? "" : ora_reader.GetString(ora_reader.GetOrdinal("error_message_en"));
                                    result.success = false; // If there are rows, it means there was an error
                                }
                        }
                    }
                    #endregion

                    await connection.CloseAsync();
                    await connection.DisposeAsync();
                }
            }




            catch (Exception exc)
            {
                result = new GeneralResult
                {
                    success = false,
                    error = new Error
                    {
                        errorCode = -1,
                        errorMessageGe = "ზოგადი შეცდომა სერვისის გამოძახებისას",
                        errorMessageEn = $"General error. Error details: {exc.Message}"
                    }
                };
            }

            return result;
        }

        public async Task<GetLoanScheduleResult> GetLoanSchedule(int loanId)
        {
            GetLoanScheduleResult result = new GetLoanScheduleResult
            {
                LoanId = loanId,
                Schedules = new List<Schedule>()
            };
            #region GetLoanSchedule
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {

                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.get_loan_schedule";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;
                        cmd.Parameters.Add("p_agr_no", OracleDbType.Int32).Value = loanId;
                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }
                        await cmd.ExecuteNonQueryAsync();

                        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            while (await reader.ReadAsync())
                            {
 
                               var schedule = new Schedule
                                {
                                    CalculationDate = reader.GetDateTime(reader.GetOrdinal("calc_date")),
                                    PaymentDate = reader.GetDateTime(reader.GetOrdinal("pay_date")),
                                    Remainder = reader.GetDecimal(reader.GetOrdinal("remainder")),
                                    BaseAmount = reader.GetDecimal(reader.GetOrdinal("base")),
                                    InterestAmount = reader.GetDecimal(reader.GetOrdinal("int")),
                                    Insurence = reader.GetDecimal(reader.GetOrdinal("insurance")),
                                    PaymentAmount = reader.GetDecimal(reader.GetOrdinal("payment_Amount")),
                                    InitialAmount = reader.GetDecimal(reader.GetOrdinal("initial_Amount")),
                                   

                                   Fee = reader.GetDecimal(reader.GetOrdinal("fee"))  
                                };

                                result.Schedules.Add(schedule);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                result.Error = new Error { errorCode = -1, errorMessageGe = exc.Message, errorMessageEn = exc.Message };
                throw;
            }
            #endregion
            return result;
        }

        public async Task<GetMereTrancheDetailsResult> GetMereTrancheDetails(int loanId)
        {
            GetMereTrancheDetailsResult result = new GetMereTrancheDetailsResult
            {
                LoanId = loanId,
                
                TrancheSchedules = new List<TrancheSchedules>()
            };
            #region GetMereTrancheDetailsResult
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {

                    using (OracleCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.GET_MERE_SCHEDULE_DETAILS";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;
                        cmd.Parameters.Add("p_agr_no", OracleDbType.Int32).Value = loanId;
                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;
                        if (connection.State != ConnectionState.Open)
                        {
                            await connection.OpenAsync();
                        }
                        await cmd.ExecuteNonQueryAsync();

                        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection))
                        {
                            while (await reader.ReadAsync())
                            {

                                var trancheSchedule = new TrancheSchedules
                                {   TrancheId =  reader.GetInt32(reader.GetOrdinal("TRANCHE_ID")),
                                    MerchantName = await reader.IsDBNullAsync("MERCHANT_NAME") ? null : reader.GetString(reader.GetOrdinal("MERCHANT_NAME")),
                                    TransactionDate = await reader.IsDBNullAsync("VALUE_DATE") ? null : reader.GetDateTime(reader.GetOrdinal("VALUE_DATE")),
                                    TransactionAmount = await reader.IsDBNullAsync("TRANCHE_AMOUNT") ? null : reader.GetDecimal(reader.GetOrdinal("TRANCHE_AMOUNT")),
                                    RemainingDebt = await reader.IsDBNullAsync("REMAINING_DEBT") ? null : reader.GetDecimal(reader.GetOrdinal("REMAINING_DEBT")),
                                    InitialSchedule =  new InitialSchedule
                                    {
                                        RecommendedMonths =  reader.GetInt32(reader.GetOrdinal("REC_MONTHS")),
                                        RecommendedMonthlyPayment =  reader.GetDecimal(reader.GetOrdinal("SCHEDULED_DEBT"))
                                    },

                                    RemainingSchedule = new RemainingSchedule
                                    {
                                        RemainingMonths =   reader.GetInt32(reader.GetOrdinal("REAMING_MONTHS")),                                        
                                        RemainingMonthlyPayment =  reader.GetDecimal(reader.GetOrdinal("SCHEDULED_DEBT"))
                                    }

                                };
                               
                                result.TrancheSchedules.Add(trancheSchedule);
                                result.ProlongCommission = reader.GetDecimal(reader.GetOrdinal("prolong_com"));
                                result.PayDate = await reader.IsDBNullAsync("PAY_DT") ? null : reader.GetDateTime(reader.GetOrdinal("PAY_DT"));
                                result.TotalRecommendedPayment = reader.GetDecimal(reader.GetOrdinal("shcedule_debt_sum"));

                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                result.Error = new Error { errorCode = -1, errorMessageGe = exc.Message, errorMessageEn = exc.Message };
                throw;
            }
            #endregion
            return result;
        }



        public async Task<GeneralResult> ProlongMere(ProlongMereParams prolongMereParams)
        {
            GeneralResult v_result = null;
            try
            {
                using (OracleConnection oracle_conn = new OracleConnection(connectionString))
                {
                    #region AddCurrencyToAccount
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "INTEGRATIONS.PROLONG_MERE";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.BindByName = true;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("return_value", OracleDbType.Int32).Direction = ParameterDirection.ReturnValue;
                        cmd.Parameters.Add("p_agr_no", OracleDbType.Int32).Value = prolongMereParams.LoanId;
                        cmd.Parameters.Add("p_tranche_id", OracleDbType.Int32).Value = prolongMereParams.TrancheId;
                        cmd.Parameters.Add("p_prolong_month", OracleDbType.Varchar2).Value = prolongMereParams.ProlongationMonths; 
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
        
            



    


    
