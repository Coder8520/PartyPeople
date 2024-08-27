using Dapper;
using System.Data;
using Website.Models;
using Website.Persistence;

namespace Website.Repositories
{
    public class DrinksRepository : RepositoryBase
    {
        public DrinksRepository(IDbConnectionProvider connectionProvider) : base(connectionProvider)
        {}

        public async Task<IEnumerable<Drink>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                "[api].[spDrinksList]",
                parameters: null,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var drinks = await Connection.QueryAsync<Drink>(command);

            return drinks
                .OrderBy(x => x.Name)
                .ToList();
        }

        public async ValueTask<Drink?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var parameters = new
            {
                Id = id
            };

            var command = new CommandDefinition(
                "[api].[spDrinkGet]",
                parameters: parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return await Connection.QuerySingleOrDefaultAsync<Drink>(command);
        }
    }
}
