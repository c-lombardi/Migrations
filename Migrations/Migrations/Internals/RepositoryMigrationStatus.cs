using System.Collections.Generic;
using System.Linq;
using Migrations.ToBeImplemented;
using Migrations.Types;

namespace Migrations.Internals
{
    internal static class RepositoryMigrationStatus
    {
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

        internal static void StartMigration(
            IRepository repository, 
            RepositoryMigration migrationToStart)
        {
            repository.UpsertMigration(migrationToStart);
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