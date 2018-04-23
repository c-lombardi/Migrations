using Migrations.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Migrations.Internals
{
    internal static class MigrationHelpers
    {
        internal static MigrationVersion GetMaxVersionOrDefault(IEnumerable<RepositoryMigration> repositoryMigrations)
        {
            return repositoryMigrations.Any()
                ? repositoryMigrations.Max(migration => migration.Version)
                : MigrationVersion.Default();
        }

        internal static IEnumerable<Migration> GetMigrationsToBeAppliedAscending(
            MigrationVersion lastVersionApplied,
            IEnumerable<Migration> allMigrationsToBeApplied)
        {
            return allMigrationsToBeApplied
                .Where(migration => migration.Version > lastVersionApplied)
                .OrderBy(migration => migration.Version);
        }

        internal static void StartMigration(
            Action<RepositoryMigration> addMigration,
            RepositoryMigration migrationToStart)
        {
            addMigration(migrationToStart);
        }

        internal static void CompleteMigration(
            Action<RepositoryMigration> upsertMigration,
            RepositoryMigration migrationToComplete)
        {
            migrationToComplete.CompleteMigration();
            upsertMigration(migrationToComplete);
        }

        internal static void FailMigration(
            Action<RepositoryMigration> deleteMigration,
            RepositoryMigration migrationToComplete)
        {
            deleteMigration(migrationToComplete);
        }
    }
}
