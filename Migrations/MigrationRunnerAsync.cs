using Migrations.Internals;
using Migrations.ToBeImplemented;
using Migrations.Types;
using Migrations.Types.ToBeImplemented;
using System;
using System.Threading.Tasks;

namespace Migrations
{
    public static class MigrationRunnerAsync
    {
        private static Task OnRollbackExceptionAsync(
            MigrationAsync migration,
            Exception exception)
        {
            throw new MigrationException(
                exception,
                migration.Version);
        }

        private static async Task OnUpgradeExceptionAsync(
            IRepositoryToMigrate repositoryToMigrate,
            IRepositoryAsync repository,
            RepositoryMigrationAsync repositoryMigration,
            Exception exception)
        {
            try
            {
                await repositoryMigration.Migration.RollbackAsync(
                    repository, 
                    repositoryToMigrate).ConfigureAwait(false);

                await MigrationHelpersAsync.FailMigrationAsync(
                    repository.DeleteMigrationAsync,
                    repositoryMigration).ConfigureAwait(false);
                throw new MigrationException(
                    exception,
                    repositoryMigration.Version);
            }
            catch (Exception rollbackEx)
            {
                await OnRollbackExceptionAsync(
                    repositoryMigration.Migration,
                    rollbackEx).ConfigureAwait(false);
            }
        }

        private static async Task ApplyMigrationAsync(
            IRepositoryToMigrate repositoryToMigrate,
            IRepositoryAsync repository,
            RepositoryMigrationAsync startedMigration)
        {
            try
            {
                await MigrationHelpersAsync.StartMigrationAsync(
                    repository.AddMigrationAsync,
                    startedMigration).ConfigureAwait(false);

                await startedMigration.Migration.UpdateAsync(
                    repository, 
                    repositoryToMigrate).ConfigureAwait(false);

                await MigrationHelpersAsync.CompleteMigrationAsync(
                    repository.UpsertMigrationAsync,
                    startedMigration).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await OnUpgradeExceptionAsync(
                    repositoryToMigrate,
                    repository,
                    startedMigration,
                    exception).ConfigureAwait(false);
            }
        }

        private static async Task ApplyMigrationsAsync(
            IRepositoryToMigrate repositoryToMigrate,
            IRepositoryAsync repository,
            MigrationSessionAsync migrationSession)
        {
            foreach (MigrationAsync migration in migrationSession.MigrationsToBeApplied)
            {
                await ApplyMigrationAsync(
                    repositoryToMigrate,
                    repository,
                    new RepositoryMigrationAsync(migration)).ConfigureAwait(false);
            }
        }

        private static Task UpdateToAsync(
            IRepositoryToMigrate repositoryToMigrate,
            IRepositoryAsync repository,
            MigrationSessionAsync migrationSession)
        {
            return ApplyMigrationsAsync(
                repositoryToMigrate,
                repository,
                migrationSession);
        }

        /// <summary>
        /// Use the migration locator to derive what migrations should be executed.
        /// Compare the migrations to be executed against the migrations applied to the repository. 
        /// Synchronously apply the migrations that were not applied to the repository in ascending order.
        /// </summary>
        /// <param name="migrationLocator">The migration locator</param>
        /// <param name="repository">The migration repository and the repository to apply the migrations to.</param>
        public static Task UpdateToLatestAsync(
            IMigrationLocatorAsync migrationLocator,
            IRepositoryAsync repository)
        {
            return UpdateToLatestAsync(
                migrationLocator,
                repository,
                repository as IRepositoryToMigrate);
        }

        /// <summary>
        /// Use the migration locator to derive what migrations should be executed.
        /// Compare the migrations to be executed against the migrations applied to the repository. 
        /// Synchronously apply the migrations that were not applied to the repository to migrate in ascending order.
        /// </summary>
        /// <param name="migrationLocator">The migration locator</param>
        /// <param name="repository">The migration repository</param>
        /// <param name="repositoryToMigrate">The repository to apply the migrations to.</param>
        public static async Task UpdateToLatestAsync(
            IMigrationLocatorAsync migrationLocator,
            IRepositoryAsync repository,
            IRepositoryToMigrate repositoryToMigrate)
        {
            MigrationSessionAsync migrationSession = 
                await MigrationSessionHelpersAsync.TryStartMigrationSession(
                    repository.AddMigrationSessionAsync,
                    MigrationSessionHelpersAsync.MigrationSessionIsAvailableToExecuteAsync(await repository.GetMigrationSessionsAsync().ConfigureAwait(false)),
                    MigrationHelpersAsync.GetMigrationsToBeAppliedAscending(
                        MigrationHelpersAsync.GetMaxVersionOrDefault(await repository.GetMigrationsAsync().ConfigureAwait(false)),
                        await migrationLocator.GetAllMigrationsAsync().ConfigureAwait(false))).ConfigureAwait(false);

            if (migrationSession != null)
            {
                try
                {
                    await UpdateToAsync(
                        repositoryToMigrate,
                        repository,
                        migrationSession).ConfigureAwait(false);
                    await MigrationSessionHelpersAsync.CompleteMigrationSessionAsync(
                        repository.UpsertMigrationSessionAsync,
                        migrationSession).ConfigureAwait(false);
                }
                catch (MigrationException migrationException)
                {
                    await MigrationSessionHelpersAsync.FailMigrationSessionAsync(
                        repository.UpsertMigrationSessionAsync,
                        migrationSession,
                        migrationException).ConfigureAwait(false);
                    throw migrationException;
                }
            }
        }
    }
}