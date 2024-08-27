using Dapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Data;
using System.Threading;
using Website.Models;
using Website.Persistence;

namespace Website.Repositories
{
    public class EmployeeEventsRepository : RepositoryBase
    {
        public EmployeeEventsRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        /// <summary>
        ///     Insert attending event for employee
        /// </summary>
        public async Task InsertAsync(int eventId, int employeeId, CancellationToken cancellationToken = default)
        {
            var parameters = new
            {
                employeeId,
                eventId
            };

            var command = new CommandDefinition(
                "[api].[spEmployeeEventsInsert]", // TO DO
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await Connection.ExecuteAsync(command);
        }

        /// <summary>
        ///     Delete attending event for employee
        /// </summary>
        public async Task DeleteAsync(int eventId, int employeeId, CancellationToken cancellationToken = default)
        {
            var parameters = new
            {
                employeeId,
                eventId
            };

            var command = new CommandDefinition(
                "[api].[spEmployeeEventsDelete]", // TO DO
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            await Connection.ExecuteAsync(command);
        }

        /// <summary>
        ///     Determines if a Employee Event exists
        /// </summary>
        public async ValueTask<bool> ExistsAsync(int eventId, int employeeId, CancellationToken cancellationToken = default)
        {
            var parameters = new
            {
                employeeId,
                eventId
            };

            var command = new CommandDefinition(
                "[api].[spEmployeeEventsExists]",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return await Connection.ExecuteScalarAsync<bool>(command);
        }
    }
}
