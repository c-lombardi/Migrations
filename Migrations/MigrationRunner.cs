using Migrations.Internals;
using System;
using Migrations.Types;
using Migrations.ToBeImplemented;
using Migrations.Types.ToBeImplemented;

namespace Migrations
{
    public static class MigrationRunner
    {
        private static void OnRollbackException(
            Migration migration,
            Exception exception)
        {
            throw new MigrationException(
                exception,
                migration.Version);
        }

        private static void OnUpgradeException(
            IRepositoryToMigrate repositoryToMigrate,
            IRepository repository,
            RepositoryMigration repositoryMigration,
            Exception exception)
        {
            try
            {
                repositoryMigration.Migration.Rollback(repositoryToMigrate);
                MigrationHelpers.FailMigration(
                    repository.DeleteMigration,
                    repositoryMigration);
                throw new MigrationException(
                    exception,
                    repositoryMigration.Version);
            }
            catch (Exception rollbackEx)
            {
                OnRollbackException(
                    repositoryMigration.Migration,
                    rollbackEx);
            }
        }

        private static void ApplyMigration(
            IRepositoryToMigrate repositoryToMigrate,
            IRepository repository,
            RepositoryMigration startedMigration)
        {
            try
            {
                MigrationHelpers.StartMigration(
                    repository.AddMigration,
                    startedMigration);

                startedMigration.Migration.Update(repositoryToMigrate);

                MigrationHelpers.CompleteMigration(
                    repository.UpsertMigration,
                    startedMigration);
            }
            catch (Exception exception)
            {
                OnUpgradeException(
                    repositoryToMigrate,
                    repository,
                    startedMigration,
                    exception);
            }
        }

        private static void ApplyMigrations(
            IRepositoryToMigrate repositoryToMigrate,
            IRepository repository,
            MigrationSession migrationSession)
        {
            foreach (Migration migration in migrationSession.MigrationsToBeApplied)
            {
                ApplyMigration(
                    repositoryToMigrate,
                    repository,
                    new RepositoryMigration(migration));
            }
        }

        private static void UpdateTo(
            IRepositoryToMigrate repositoryToMigrate,
            IRepository repository,
            MigrationSession migrationSession)
        {
            ApplyMigrations(
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
        public static void UpdateToLatest(
            IMigrationLocator migrationLocator,
            IRepository repository)
        {
            UpdateToLatest(
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
        public static void UpdateToLatest(
            IMigrationLocator migrationLocator,
            IRepository repository,
            IRepositoryToMigrate repositoryToMigrate)
        {
            MigrationSession migrationSession;
            if (MigrationSessionHelpers.TryStartMigrationSession(
                repository.AddMigrationSession,
                MigrationSessionHelpers.MigrationSessionIsAvailableToExecute(repository.GetMigrationSessions()),
                MigrationHelpers.GetMigrationsToBeAppliedAscending(
                    MigrationHelpers.GetMaxVersionOrDefault(repository.GetMigrations()),
                    migrationLocator.GetAllMigrations()),
                out migrationSession))
            {
                try
                {
                    UpdateTo(
                        repositoryToMigrate,
                        repository,
                        migrationSession);
                    MigrationSessionHelpers.CompleteMigrationSession(
                        repository.UpsertMigrationSession,
                        migrationSession);
                }
                catch (MigrationException migrationException)
                {
                    MigrationSessionHelpers.FailMigrationSession(
                        repository.UpsertMigrationSession,
                        migrationSession,
                        migrationException);
                    throw migrationException;
                }
            }
        }
    }
}