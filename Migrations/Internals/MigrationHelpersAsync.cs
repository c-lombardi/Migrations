using Migrations.Types;
using Migrations.Types.ToBeImplemented;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Migrations.Internals
{
    internal static class MigrationHelpersAsync
    {
        internal static MigrationVersion GetMaxVersionOrDefault(IEnumerable<RepositoryMigrationAsync> repositoryMigrations)
        {
            return repositoryMigrations.Any()
                ? repositoryMigrations.Max(migration => migration.Version)
                : MigrationVersion.Default();
        }

        internal static IEnumerable<MigrationAsync> GetMigrationsToBeAppliedAscending(
            MigrationVersion lastVersionApplied,
            IEnumerable<MigrationAsync> allMigrationsToBeApplied)
        {
            return allMigrationsToBeApplied
                .Where(migration => migration.Version > lastVersionApplied)
                .OrderBy(migration => migration.Version);
        }

        internal static Task StartMigrationAsync(
            Func<RepositoryMigrationAsync, Task> addMigration,
            RepositoryMigrationAsync migrationToStart)
        {
            return addMigration(migrationToStart);
        }

        internal static Task CompleteMigrationAsync(
            Func<RepositoryMigrationAsync, Task> upsertMigration,
            RepositoryMigrationAsync migrationToComplete)
        {
            migrationToComplete.CompleteMigration();
            return upsertMigration(migrationToComplete);
        }

        internal static Task FailMigrationAsync(
            Func<RepositoryMigrationAsync, Task> deleteMigration,
            RepositoryMigrationAsync migrationToComplete)
        {
            return deleteMigration(migrationToComplete);
        }
    }
}
