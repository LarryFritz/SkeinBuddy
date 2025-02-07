using Dapper;
using Npgsql;
using SkeinBuddy.AI;
using SkeinBuddy.DataAccess.Factories;
using SkeinBuddy.Enumerations;
using SkeinBuddy.Models;
using SkeinBuddy.Queries;
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
        private readonly YarnNameEmbeddingRepository _yarnNameEmbeddingRepository;
        private readonly XenovaEmbeddingService _xenoviaEmbeddingService;

        public YarnRepository(ConnectionFactory connectionFactory, YarnNameEmbeddingRepository yarnNameEmbeddingRepository, XenovaEmbeddingService xenovaEmbeddingService) 
        {
            _connectionFactory = connectionFactory;
            _yarnNameEmbeddingRepository = yarnNameEmbeddingRepository;
            _xenoviaEmbeddingService = xenovaEmbeddingService;
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

        public async Task<PagedResult<Yarn>> QueryAsync(YarnQuery? query)
        {
            query ??= new YarnQuery();

            DynamicParameters parameters = new DynamicParameters();

            string sql = $@"
                SELECT y.* 
                FROM yarn y
                WHERE 
                    1 = 1                
            ";

            if (query.Id.HasValue)
            {
                parameters.Add(nameof(query.Id), query.Id.Value, DbType.Guid);
                sql += $" AND y.{nameof(Yarn.Id)} = @{nameof(query.Id)}";
            }

            if (query.BrandId.HasValue)
            {
                parameters.Add(nameof(query.BrandId), query.BrandId.Value, DbType.Guid);
                sql += $" AND y.{nameof(Yarn.BrandId)} = @{nameof(query.BrandId)}";
            }

            if(query.Weight.HasValue)
            {
                parameters.Add(nameof(query.Weight), query.Weight.Value, DbType.Int32);
                sql += $" AND y.{nameof(Yarn.Weight)} = @{nameof(query.Weight)}";
            }

            if(!string.IsNullOrWhiteSpace(query.Search))
            {
                parameters.Add(nameof(query.Search), $"%{query.Search}%", DbType.String);
                sql += $" AND y.{nameof(Yarn.Name)} ILIKE @{nameof(query.Search)}";
            }

            if (query.Sorts != null && query.Sorts.Count() > 0)
            {
                sql += " ORDER BY ";

                int index = 0;
                foreach (string sortValue in query.Sorts)
                {
                    if(index > 0)
                    {
                        sql += ", ";
                    }

                    string[] sortParts = sortValue.Split(':');
                    if (!string.IsNullOrWhiteSpace(sortValue) && sortParts.Length == 2 && Enum.TryParse(sortParts[0], true, out YarnQuery.SortColumns column) && Enum.TryParse(sortParts[1], true, out SortDirection direction))
                    {
                        sql += $" {Enum.GetName(column)} {(direction == SortDirection.Ascending ? "ASC" : "DESC")}";
                    }

                    index++;
                }
            }

            PagedResult<Yarn> pagedResult = new PagedResult<Yarn>();
            pagedResult.Skip = query.Skip;
            pagedResult.Take = query.Take;

            using (NpgsqlConnection connection = _connectionFactory.GetPostgresConnection())
            {
                pagedResult.TotalCount =  await connection.QuerySingleOrDefaultAsync<int>($"SELECT COUNT(*) FROM ({sql})", parameters);
                pagedResult.Records = await connection.QueryAsync<Yarn>(sql, parameters);
            }

            return pagedResult;
        }

        public async Task<List<Yarn>> QueryNearestNeighborsByName(string name, CancellationToken cancellationToken)
        {
            float[] embedding = (await _xenoviaEmbeddingService.GenerateEmbeddingsAsync([name], cancellationToken)).First().ToArray();

            return (await _yarnNameEmbeddingRepository.QueryNearestNeighbors(embedding)).ToList();
        }

        public async Task<Yarn> CreateAsync(Yarn yarn, CancellationToken cancellationToken)
        {
            using (NpgsqlConnection connection = _connectionFactory.GetPostgresConnection())
            {
                await connection.ExecuteAsync($@"
                    INSERT INTO yarn (id, created_at, weight, name, brand_id)
                    VALUES (@Id, @CreatedAt, @Weight, @Name, @BrandId)
                ", yarn);
            }

            float[] embedding = (await _xenoviaEmbeddingService.GenerateEmbeddingsAsync([yarn.Name], cancellationToken)).First().ToArray();

            await _yarnNameEmbeddingRepository.CreateYarnNameEmbedding(yarn.Id, embedding);

            return yarn;
        }
    }
}
