using Migrations.Types;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Migrations.ToBeImplemented
{
    public interface IRepositoryAsync
    {
        /// <summary>
        /// Get all migration sessions stored in the repository.
        /// </summary>
        /// <returns>The migration sessions stored in the repository.</returns>
        Task<IEnumerable<MigrationSessionAsync>> GetMigrationSessionsAsync();

        /// <summary>
        /// Get all migrations stored in the repository.
        /// </summary>
        /// <returns>The migrations stored in the repository.</returns>
        Task<IEnumerable<RepositoryMigrationAsync>> GetMigrationsAsync();

        /// <summary>
        /// Insert a migration session into the repository.
        /// </summary>
        /// <param name="migrationSession">The migration session to store into the repository.</param>
        Task AddMigrationSessionAsync(MigrationSessionAsync migrationSession);

        /// <summary>
        /// Insert a migration into the repository.
        /// </summary>
        /// <param name="migration">The migration to store into the repository.</param>
        Task AddMigrationAsync(RepositoryMigrationAsync migration);

        /// <summary>
        /// Insert or Update a migration session into the repository.
        /// </summary>
        /// <param name="migration">The migration session to insert or update into the repository.</param>
        Task UpsertMigrationSessionAsync(MigrationSessionAsync migration);

        /// <summary>
        /// Insert or update a migration into the repository
        /// </summary>
        /// <param name="migration">The migration to insert or update into the repository.</param>
        Task UpsertMigrationAsync(RepositoryMigrationAsync migration);

        /// <summary>
        /// Delete a migration from the repository.
        /// </summary>
        /// <param name="migration">The migration to delete from the repository.</param>
        Task DeleteMigrationAsync(RepositoryMigrationAsync migration);
    }
}
