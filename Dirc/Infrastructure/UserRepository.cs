using Dirc.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dirc.Infrastructure
{
    public class UserRepository : RepositoryBase<User>
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<Guid> InsertAsync(User entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            string sql = @"
                INSERT INTO Users (Id, Username, Password, Role, CreatedAt)
                VALUES (@Id, @Username, @Password, @Role, @CreatedAt)
                RETURNING Id;";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", entity.Id ?? Guid.NewGuid() },
                { "@Username", entity.Username },
                { "@Password", entity.Password },
                { "@Role", entity.Role },
                { "@CreatedAt", entity.CreatedAt == default ? DateTime.UtcNow : entity.CreatedAt }
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

        public override async Task UpdateAsync(Guid id, User entity)
        {
            string sql = @"
                UPDATE Users
                SET Username = @Username,
                    Password = @Password,
                    Role = @Role,
                    CreatedAt = @CreatedAt
                WHERE Id = @Id;";

            var parameters = new Dictionary<string, object>
            {
                { "@Id", id },
                { "@Username", entity.Username },
                { "@Password", entity.Password },
                { "@Role", entity.Role },
                { "@CreatedAt", entity.CreatedAt }
            };

            await ExecuteSqlAsync(sql, parameters);
        }

        public override async Task DeleteAsync(Guid id)
        {
            string sql = "DELETE FROM Users WHERE Id = @Id;";
            var parameters = new Dictionary<string, object> { { "@Id", id } };
            await ExecuteSqlAsync(sql, parameters);
        }

        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            string sql = "SELECT * FROM Users;";
            return await ExecuteSqlReaderAsync(sql);
        }

        public override async Task<User> GetByIdAsync(Guid id)
        {
            string sql = "SELECT * FROM Users WHERE Id = @Id;";
            var parameters = new Dictionary<string, object> { { "@Id", id } };

            var result = await ExecuteSqlReaderAsync(sql, parameters);
            return result.SingleOrDefault();
        }

        protected override User GetEntityFromReader(NpgsqlDataReader reader)
        {
            return new User
            {
                Id = reader["Id"] as Guid?,
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(),
                Role = reader["Role"].ToString(),
                CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
            };
        }
    }
}
