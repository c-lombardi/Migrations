using System.Collections.Generic;
using System.Linq;
using Migrations.ToBeImplemented;
using Migrations.Types;

namespace Migrations.Internals
{
    internal static class RepositoryMigrationStatus
    {
        private static bool MigrationIsExecuting(IRepository repository)
        {
            return repository.GetMigrations()
                .Any(session => session.CompletedOn == null);
        }

        internal static IEnumerable<Migration> GetMigrationsApplied(
            IRepository repository,
            IMigrationLocator migrationLocator)
        {
            IEnumerable<RepositoryMigration> appliedMigrations =
                repository.GetMigrations();

            MigrationVersion lastVersionApplied = appliedMigrations.Any()
                ? appliedMigrations.Max(migration => migration.Version)
                : MigrationVersion.Default();

            return migrationLocator.GetAllMigrations()
                .Where(migration => migration.Version > lastVersionApplied);
        }

        internal static bool TryStartMigration(
            IRepository repository,
            RepositoryMigration migrationToStart)
        {
            bool migrationStarted = !MigrationIsExecuting(repository);

            if (migrationStarted)
                return false;

            repository.AddMigration(migrationToStart);
            return true;
        }

        internal static void CompleteMigration(
            IRepository repository,
            RepositoryMigration migrationToComplete)
        {
            migrationToComplete.CompleteMigration();
            repository.UpsertMigration(migrationToComplete);
        }

        internal static void FailMigration(
            IRepository repository,
            RepositoryMigration migrationToComplete)
        {
            repository.DeleteMigration(migrationToComplete);
        }
    }
}