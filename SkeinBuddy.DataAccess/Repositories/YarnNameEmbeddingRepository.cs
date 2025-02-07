using Dapper;
using Npgsql;
using Pgvector;
using SkeinBuddy.DataAccess.Factories;
using SkeinBuddy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeinBuddy.DataAccess.Repositories
{
    public class YarnNameEmbeddingRepository
    {
        private readonly ConnectionFactory _connectionFactory;
        public YarnNameEmbeddingRepository(ConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CreateYarnNameEmbedding(Guid yarnId, float[] embedding)
        {
            using (NpgsqlConnection connection = _connectionFactory.GetPostgresConnection())
            {
                await connection.ExecuteAsync($@"
                    INSERT INTO yarn_name_embedding (yarn_id, embedding) VALUES (@yarnId, @embedding)
                ", new {yarnId = yarnId, embedding = new Vector(embedding)});
            }
        }



        public async Task<IEnumerable<Yarn>> QueryNearestNeighbors(float[] embedding)
        {
            using (NpgsqlConnection connection = _connectionFactory.GetPostgresConnection())
            {
                return await connection.QueryAsync<Yarn>($@"
                    SELECT y.*
                    FROM public.yarn_name_embedding yne
                    INNER JOIN public.yarn y ON y.id = yne.yarn_id
                    ORDER BY yne.embedding <-> @embedding 
                    LIMIT 5
                ", new { embedding = new Vector(embedding) });
            }
            //var items = conn.Query<Item>("SELECT * FROM items ORDER BY embedding <-> @embedding LIMIT 5", new { embedding });
        }
    }
}
