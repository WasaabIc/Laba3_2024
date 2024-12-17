using Dirc.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dirc.Infrastructure
{
    public abstract class RepositoryBase<T> : IRepository<T>
    {
        protected readonly string connectionString;

        public RepositoryBase(IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Ошибка конфигурации: параметр ConnectionStrings:DefaultConnection отсутствует.");
        }

        public abstract Task<Guid> InsertAsync(T entity);
        public abstract Task UpdateAsync(Guid id, T entity);
        public abstract Task DeleteAsync(Guid id);
        public abstract Task<IEnumerable<T>> GetAllAsync();
        public abstract Task<T> GetByIdAsync(Guid id);

        protected abstract T GetEntityFromReader(NpgsqlDataReader reader);

        protected virtual async Task<IEnumerable<T>> ExecuteSqlReaderAsync(string sql, Dictionary<string, object> parameters = null)
        {
            var result = new List<T>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    AddParameters(command, parameters);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(GetEntityFromReader(reader));
                        }
                    }
                }
            }
            return result;
        }

        protected virtual async Task ExecuteSqlAsync(string sql, Dictionary<string, object> parameters = null)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(sql, connection))
                {
                    AddParameters(command, parameters);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private void AddParameters(NpgsqlCommand command, Dictionary<string, object> parameters)
        {
            if (parameters == null) return;
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value);
            }
        }
    }
}
