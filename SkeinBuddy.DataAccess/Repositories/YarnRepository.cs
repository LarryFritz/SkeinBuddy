using Dapper;
using Npgsql;
using SkeinBuddy.DataAccess.Factories;
using SkeinBuddy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.DataAccess.Repositories
{
    public class YarnRepository
    {
        private readonly ConnectionFactory _connectionFactory;
        public YarnRepository(ConnectionFactory connectionFactory) 
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Yarn>> GetAllAsync()
        {
            using (NpgsqlConnection connection = _connectionFactory.GetPostgresConnection())
            {
                return await connection.QueryAsync<Yarn>($@"
                    SELECT * FROM yarn
                ");
            }
        }

        public async Task<Yarn> GetByIdAsync(Guid id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add(nameof(id), id, DbType.Guid);

            using (NpgsqlConnection connection = _connectionFactory.GetPostgresConnection())
            {
                return await connection.QueryFirstAsync<Yarn>(@"SELECT * FROM yarn WHERE id = @id", parameters);
            }
        }
    }
}
