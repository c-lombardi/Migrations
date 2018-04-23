using Migrations.Internals;
using System;
using Migrations.ToBeImplemented;
using Migrations.Types;

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
            IRepository repository,
            RepositoryMigration repositoryMigration,
            Exception exception)
        {
            try
            {
                repositoryMigration.Migration.Rollback(repository);
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

        private static void ApplyMigration(IRepository repository, RepositoryMigration startedMigration)
        {
            try
            {
                if (MigrationHelpers.TryStartMigration(
                    repository.AddMigration,
                    startedMigration,
                    MigrationHelpers.MigrationIsAvailableToExecute(repository.GetMigrations())))
                {
                    startedMigration.Migration.Update(repository);

                    MigrationHelpers.CompleteMigration(
                        repository.UpsertMigration,
                        startedMigration);
                }
            }
            catch (Exception exception)
            {
                OnUpgradeException(
                    repository,
                    startedMigration,
                    exception);
            }
        }

        private static void ApplyMigrations(
            IRepository repository,
            MigrationSession migrationSession)
        {
            foreach (Migration migration in migrationSession.MigrationsToBeApplied)
            {
                ApplyMigration(
                    repository,
                    new RepositoryMigration(migration));
            }
        }

        private static void UpdateTo(
            IRepository repository,
            MigrationSession migrationSession)
        {
            ApplyMigrations(
                repository,
                migrationSession);
        }

        /// <summary>
        /// Derive what migrations should be executed from the migration locator compared to the current state of migrations applied from the repository and apply them in order of version ascending.
        /// </summary>
        /// <param name="migrationLocator">The Migration Locator</param>
        /// <param name="repository">The Migration Repository</param>
        public static void UpdateToLatest(
            IMigrationLocator migrationLocator,
            IRepository repository)
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
                    UpdateTo(repository, migrationSession);
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