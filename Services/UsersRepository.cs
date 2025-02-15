using Integrations.Model;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class UsersRepository : IUsersRepository
    {
        public readonly string connectionString;
        private int? _commandTimeout;


        public UsersRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("OracleDBConnection");
            _commandTimeout = 180;
        }

        public async Task<bool> AreCredentialsValid(string userName, string password)
        {
            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                bool v_return_value;

                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "api_user_security.valid_user";
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("return_value", OracleDbType.Decimal).Direction = ParameterDirection.ReturnValue;
                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = userName;
                    cmd.Parameters.Add("p_password", OracleDbType.Varchar2).Value = password;


                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    await cmd.ExecuteNonQueryAsync();


                    v_return_value = Convert.ToBoolean((decimal)(OracleDecimal)cmd.Parameters["return_value"].Value);

                }

                return v_return_value;
            }

        }

        public async Task<List<UserRole>> GetUserRoles(string userName)
        {
            List<UserRole> v_user_roles = null;

            using (OracleConnection oracle_conn = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = oracle_conn.CreateCommand())
                {
                    cmd.CommandText = "api_user_security.get_user_roles";
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (_commandTimeout != null)
                        cmd.CommandTimeout = _commandTimeout.Value;

                    cmd.Parameters.Add("p_username", OracleDbType.Varchar2).Value = userName;
                    cmd.Parameters.Add("P_RECORDSET", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    if (oracle_conn.State != ConnectionState.Open)
                    {
                        await oracle_conn.OpenAsync();
                    }

                    DbDataReader ora_reader = await cmd.ExecuteReaderAsync();

                    if (ora_reader.HasRows)
                    {
                        v_user_roles = new();
                        while (await ora_reader.ReadAsync())
                        {
                            UserRole v_user_role = new();
                            v_user_role.UserName = userName;
                            v_user_role.RoleName = ora_reader.GetString("role_name");

                            v_user_roles.Add(v_user_role);
                        }
                    }



                }

                return v_user_roles;
            }

        }


    }
}
