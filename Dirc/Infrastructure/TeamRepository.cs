using Dirc.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dirc.Infrastructure
{
    public class TeamRepository : RepositoryBase<Team>
    {
        public TeamRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<Guid> InsertAsync(Team entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            string sql = @"
                INSERT INTO Teams (Id, TeamName)
                VALUES (@Id, @TeamName)
                RETURNING Id;";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id ?? Guid.NewGuid() },
                { "@TeamName", entity.TeamName }
            };

            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    foreach (var param in parameters)
                        command.Parameters.AddWithValue(param.Key, param.Value);

                    var result = await command.ExecuteScalarAsync();
                    return (Guid)result;
                }
            }
        }

        public override async Task UpdateAsync(Guid id, Team entity)
        {
            string sql = @"
                UPDATE Teams
                SET TeamName = @TeamName
                WHERE Id = @Id;";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id },
                { "@TeamName", entity.TeamName }
            };

            await ExecuteSqlAsync(sql, parameters);
        }

        public override async Task DeleteAsync(Guid id)
        {
            string sql = "DELETE FROM Teams WHERE Id = @Id;";
            var parameters = new Dictionary<string, object> { { "@Id", id } };
            await ExecuteSqlAsync(sql, parameters);
        }

        public override async Task<IEnumerable<Team>> GetAllAsync()
        {
            string sql = "SELECT * FROM Teams;";
            return await ExecuteSqlReaderAsync(sql);
        }

        public override async Task<Team> GetByIdAsync(Guid id)
        {
            string sql = "SELECT * FROM Teams WHERE Id = @Id;";
            var parameters = new Dictionary<string, object> { { "@Id", id } };

            var result = await ExecuteSqlReaderAsync(sql, parameters);
            return result.SingleOrDefault();
        }

        protected override Team GetEntityFromReader(NpgsqlDataReader reader)
        {
            return new Team
            {
                Id = reader["Id"] as Guid?,
                TeamName = reader["TeamName"].ToString()
            };
        }
    }
}
