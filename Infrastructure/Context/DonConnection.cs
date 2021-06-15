using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace Infrastructure.Context
{
    public class DonConnection : IDonConnection
    {
        private readonly IConfiguration _config;

        public DonConnection(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection GetConnection()
        {
            var connectionString = _config.GetConnectionString("DonConnectionString");
            var conn = new OracleConnection(connectionString);
            return conn;
        }
    }
}