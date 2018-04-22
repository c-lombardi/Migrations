using Migrations.Internals;
using System;
using Migrations.ToBeImplemented;
using Migrations.Types;

namespace Migrations
{
    public static class MigrationRunner
    {
        public static void UpdateToLatest(
            IMigrationLocator migrationLocator,
            IRepository repository)
        {
            MigrationSession migrationSession;
            if (RepositoryMigrationSession.TryStartMigrationSession(
                repository,
                RepositoryMigrationStatus.GetMigrationsApplied(
                    repository,
                    migrationLocator),
                out migrationSession))
            {
                try
                {
                    UpdateTo(repository, migrationSession);
                    RepositoryMigrationSession.CompleteMigrationSession(
                        repository,
                        migrationSession);
                }
                catch (MigrationException migrationException)
                {
                    RepositoryMigrationSession.FailMigrationSession(
                        repository,
                        migrationSession,
                        migrationException);
                    throw migrationException;
                }
            }
        }

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
                RepositoryMigrationStatus.FailMigration(
                    repository,
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
                if (RepositoryMigrationStatus.TryStartMigration(
                    repository,
                    startedMigration))
                {
                    startedMigration.Migration.Update(repository);

                    RepositoryMigrationStatus.CompleteMigration(
                        repository,
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
    }
}