using Migrations.Types;
using System.Collections.Generic;

namespace Migrations.ToBeImplemented
{
    public interface IRepository
    {
        /// <summary>
        /// Get all migration sessions stored in the repository.
        /// </summary>
        /// <returns>The migration sessions stored in the repository.</returns>
        IEnumerable<MigrationSession> GetMigrationSessions();

        /// <summary>
        /// Get all migrations stored in the repository.
        /// </summary>
        /// <returns>The migrations stored in the repository.</returns>
        IEnumerable<RepositoryMigration> GetMigrations();

        /// <summary>
        /// Insert a migration session into the repository.
        /// </summary>
        /// <param name="migrationSession">The migration session to store into the repository.</param>
        void AddMigrationSession(MigrationSession migrationSession);

        /// <summary>
        /// Insert a migration into the repository.
        /// </summary>
        /// <param name="migration">The migration to store into the repository.</param>
        void AddMigration(RepositoryMigration migration);

        /// <summary>
        /// Insert or Update a migration session into the repository.
        /// </summary>
        /// <param name="migration">The migration session to insert or update into the repository.</param>
        void UpsertMigrationSession(MigrationSession migration);

        /// <summary>
        /// Insert or update a migration into the repository
        /// </summary>
        /// <param name="migration">The migration to insert or update into the repository.</param>
        void UpsertMigration(RepositoryMigration migration);

        /// <summary>
        /// Delete a migration from the repository.
        /// </summary>
        /// <param name="migration">The migration to delete from the repository.</param>
        void DeleteMigration(RepositoryMigration migration);
    }
}
