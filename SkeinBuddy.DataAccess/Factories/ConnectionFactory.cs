using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.DataAccess.Factories
{
    public class ConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public ConnectionFactory(IConfiguration configuration) {
            _configuration = configuration;
        }

        public NpgsqlConnection GetPostgresConnection()
        {
            NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(_configuration.GetConnectionString("Postgres"));
            builder.UseVector();
            return builder.Build().CreateConnection();
        }
    }
}
