using Integrations.Model;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class ScoringRepository : IScoringRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public ScoringRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;

        }
        public async  Task<ScoringData> GetScoringData(string personalNo)
        {
            ScoringData v_scoring_data = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region get_scoring_data
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "GET_SCORING_DATA";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("P_PERSONAL_NO", OracleDbType.Varchar2).Value = personalNo;
                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                    if (ora_reader.HasRows)
                    {
                        v_scoring_data = new ScoringData();
                        while (await ora_reader.ReadAsync())
                        {
                            v_scoring_data.IsInsider = Convert.ToBoolean(ora_reader.GetInt16("is_insider"));
                            v_scoring_data.HasRestrictions = Convert.ToBoolean(ora_reader.GetInt16("has_restrictions"));
                            v_scoring_data.IsBankEmployee = Convert.ToBoolean(ora_reader.GetInt16("is_bank_employee"));
                            v_scoring_data.IsPayroll = Convert.ToBoolean(ora_reader.GetInt16("is_payroll"));                            
                            v_scoring_data.IsBankProblemClient = Convert.ToBoolean(ora_reader.GetInt16("is_bank_problem_client"));
                            v_scoring_data.IsInCort = Convert.ToBoolean(ora_reader.GetInt16("is_in_court"));
                            v_scoring_data.IsInRejectedList = Convert.ToBoolean(ora_reader.GetInt16("is_in_rejected_list"));
                            v_scoring_data.HasRestructuringLoan = Convert.ToBoolean(ora_reader.GetInt16("has_restr_loan"));
                            v_scoring_data.HadRestructuringLoan = Convert.ToBoolean(ora_reader.GetInt16("had_restr_loan"));
                            v_scoring_data.SumUnauthorizedAmount = ora_reader.GetDecimal("sum_unauth_amount");
                            v_scoring_data.SumLiabilitiesAmount = ora_reader.GetDecimal("sum_liabilities_amount");
                            v_scoring_data.MaxBadDays = ora_reader.GetInt32("max_bad_days");
                            v_scoring_data.ActiveLoanDays = ora_reader.GetInt32("active_loan_days");
                            v_scoring_data.FirstLoanDate = await ora_reader.IsDBNullAsync("first_loan_date") ? null : ora_reader.GetDateTime("first_loan_date").ToString("MM/dd/yyyy");
                            v_scoring_data.LastRestructuringDate = await ora_reader.IsDBNullAsync("last_restruct_date") ? "" : ora_reader.GetDateTime("last_restruct_date").ToString("MM/dd/yyyy");
                            v_scoring_data.CurrentRestructuringLoanDays = await ora_reader.IsDBNullAsync("current_restr_loan_days") ? null : ora_reader.GetInt32("current_restr_loan_days");
                            v_scoring_data.PastRestructuringLoanDays = await ora_reader.IsDBNullAsync("past_restr_loan_days") ? null : ora_reader.GetInt32("past_restr_loan_days");

                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                #region get_loans_for_scoring
                if(v_scoring_data != null)
                {
                    using (OracleCommand cmd = oracle_conn.CreateCommand())
                    {
                        cmd.CommandText = "GET_LOANS_FOR_SCORING";
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (_commandTimeout != null)
                            cmd.CommandTimeout = _commandTimeout.Value;

                        cmd.Parameters.Add("P_PERSONAL_NO", OracleDbType.Varchar2).Value = personalNo;
                        cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                        if (oracle_conn.State != ConnectionState.Open)
                        {
                            await oracle_conn.OpenAsync();
                        }

                        DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                        if (ora_reader.HasRows)
                        {
                            v_scoring_data.Loans = new List<Loan>();
                            while (await ora_reader.ReadAsync())
                            {
                                Loan v_loan = new();

                                v_loan.LoanId = ora_reader.GetInt32("loan_id");
                                v_loan.AgreementNo = ora_reader.GetString("agr_id");
                                v_loan.LoanType = ora_reader.GetString("loan_type");
                                v_loan.Currency = ora_reader.GetString("currency");
                                v_loan.InterestRate = ora_reader.GetDecimal("interest_rate");
                                v_loan.MaxPayment = ora_reader.GetDecimal("max_payment");
                                v_loan.IsCollateralized = Convert.ToBoolean(ora_reader.GetInt16("is_collateralized"));
                                v_loan.OutstandingInstallments = await ora_reader.IsDBNullAsync("outstanding_installments") ? null : ora_reader.GetInt32("outstanding_installments");
                                v_loan.CurrentBadDays = ora_reader.GetInt32("current_bad_days");
                                v_loan.ClientRole = ora_reader.GetString("client_role");
                                v_loan.PrincipalAmount = ora_reader.GetDecimal("principal");
                                v_loan.InterestAmount = ora_reader.GetDecimal("interest");
                                v_loan.PenaltyAmount = ora_reader.GetDecimal("penalty");
                                v_loan.CreditLimit = await ora_reader.IsDBNullAsync("credit_limit") ? null : ora_reader.GetDecimal("credit_limit");





                                v_scoring_data.Loans.Add(v_loan);
                            }
                        }
                        await ora_reader.CloseAsync();
                    }
                }

                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
                

            }

            return v_scoring_data;
        }
    }
}
