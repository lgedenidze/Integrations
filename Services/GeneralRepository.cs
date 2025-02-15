using Integrations.Model;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class GeneralRepository : IGeneralRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;

        public GeneralRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;

        }

        public async Task<Countries> GetCountries()
        {

            Countries v_countries = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region get_countries
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.get_countries";
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
                        v_countries = new Countries();
                        v_countries.countries = new();
                        while (await ora_reader.ReadAsync())
                        {
                            Country v_country = new Country();
                            v_country.Id=  ora_reader.GetInt32("no");
                            v_country.countryNameEn = await ora_reader.IsDBNullAsync("name_lat") ? "" : ora_reader.GetString("name_lat");
                            v_country.countryNameGe = await ora_reader.IsDBNullAsync("short_name") ? "" : ora_reader.GetString("short_name");
                            v_country.countryCodeISO2 = await ora_reader.IsDBNullAsync("CODE1") ? "" : ora_reader.GetString("CODE1");
                            v_country.countryCodeISO3= await ora_reader.IsDBNullAsync("CODE2") ? "" : ora_reader.GetString("CODE2");


                            v_countries.countries.Add(v_country);
                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
            }


            return v_countries;


        }
        public async Task<Districts> GetDistricts()
        {

            Districts v_districs = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region get_countries
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.get_districts";
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
                        v_districs = new Districts();
                        v_districs.districts= new();
                        while (await ora_reader.ReadAsync())
                        {
                            District district = new District();
                            district.Id = ora_reader.GetInt32("id");
                            district.DistrictNameEn = await ora_reader.IsDBNullAsync("name_en") ? null : ora_reader.GetString("name_en");
                            district.DistrictNameGe = await ora_reader.IsDBNullAsync("name") ? null : ora_reader.GetString("name");
                            district.RegionId = await ora_reader.IsDBNullAsync("region_id") ? null : ora_reader.GetInt32("region_id");
                            v_districs.districts.Add(district);
                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
            }


            return v_districs;


        }
        public async Task<Regions> GetRegions()
        {

            Regions v_regions = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                #region get_countries
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "INTEGRATIONS.get_regions";
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
                        v_regions = new Regions();
                        v_regions.regions = new();
                        while (await ora_reader.ReadAsync())
                        {
                            Region region = new Region();
                            region.Id = ora_reader.GetInt32("id");
                            region.RegionNameEn = await ora_reader.IsDBNullAsync("name_en") ? null : ora_reader.GetString("name_en");
                            region.RegionNameGe = await ora_reader.IsDBNullAsync("name") ? null : ora_reader.GetString("name");
                             v_regions.regions.Add(region);
                        }
                    }
                    await ora_reader.CloseAsync();
                }
                #endregion

                await oracle_conn.CloseAsync();
                await oracle_conn.DisposeAsync();
            }


            return v_regions;


        }


    }
}
