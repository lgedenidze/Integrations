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
    public class CurrenciesRepository : ICurrenciesRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public CurrenciesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }

        public async Task<ExchangeRate> GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            var v_result = new ExchangeRate
            {
                rate = 2.5513m
            };

            return v_result;
        }

       
        public async Task<CurrencyExchanges> GetExchangeRates()
        {
            CurrencyExchanges currencyExchanges  = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region GetExchangeRates
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.get_cur_rates";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.BindByName = true;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                     cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                    if (ora_reader.HasRows)
                    {
                        currencyExchanges = new CurrencyExchanges();
                        currencyExchanges.currencyExchange = new();
                        while (await ora_reader.ReadAsync())
                        {
                            CurrencyExchange currencyExchange = new CurrencyExchange();
                            currencyExchange.SourceCurrency = await ora_reader.IsDBNullAsync("VAL") ? null : ora_reader.GetString("VAL");
                            currencyExchange.TargetCurrency = await ora_reader.IsDBNullAsync("BAZ") ? null : ora_reader.GetString("BAZ");
                            currencyExchange.Scale = await ora_reader.IsDBNullAsync("SCALE") ? 0 : ora_reader.GetDecimal("SCALE");
                            currencyExchange.BuyRate = await ora_reader.IsDBNullAsync("RATE_BUY") ? 0 : ora_reader.GetDecimal("RATE_BUY");
                            currencyExchange.SellRate = await ora_reader.IsDBNullAsync("RATE_SELL") ? 0 : ora_reader.GetDecimal("RATE_SELL");
                            currencyExchange.LastUpdated = await ora_reader.IsDBNullAsync("UPDATED") ? DateTime.MinValue : ora_reader.GetDateTime("UPDATED");
                            currencyExchange.ExchangeType = await ora_reader.IsDBNullAsync("COURSE_TYPE") ? null : ora_reader.GetString("COURSE_TYPE");


                            currencyExchanges.currencyExchange.Add(currencyExchange);
                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
            }


            return currencyExchanges;
        }


    }
}
