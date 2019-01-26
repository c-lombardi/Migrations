using Migrations.Types;
using Migrations.Types.ToBeImplemented;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Migrations.Internals
{
    internal static class MigrationSessionHelpersAsync
    {
        internal static bool MigrationSessionIsAvailableToExecuteAsync(IEnumerable<MigrationSessionAsync> migrationSessions)
        {
            return !migrationSessions
                .Any(session => session.CompletedOn == null
                    && session.CompletedOnVersion == MigrationVersion.Default());
        }

        internal static Task CompleteMigrationSessionAsync(
            Func<MigrationSessionAsync, Task> upsertMigrationSession,
            MigrationSessionAsync migrationSession)
        {
            migrationSession.CompleteMigrationSession();
            return upsertMigrationSession(migrationSession);
        }

        internal static Task FailMigrationSessionAsync(
            Func<MigrationSessionAsync, Task> upsertMigrationSession,
            MigrationSessionAsync migrationSession,
            MigrationException migrationException)
        {
            migrationSession.FailMigrationSession(migrationException);
            return upsertMigrationSession(migrationSession);
        }

        internal static MigrationVersion GetLastVersionOrDefault(IEnumerable<MigrationAsync> migrationsToBeApplied)
        {
            return migrationsToBeApplied.Any()
                ? migrationsToBeApplied.Max(migration => migration.Version)
                : MigrationVersion.Default();
        }

        internal static MigrationVersion GetFirstVersionOrDefault(IEnumerable<MigrationAsync> migrationsToBeApplied)
        {
            return migrationsToBeApplied.Any()
                ? migrationsToBeApplied.Min(migration => migration.Version)
                : MigrationVersion.Default();
        }

        internal static async Task<MigrationSessionAsync> TryStartMigrationSession(
            Func<MigrationSessionAsync, Task> addMigrationSession,
            bool migrationSessionStarted,
            IEnumerable<MigrationAsync> migrationsToBeApplied)
        {
            MigrationSessionAsync migrationSession;
            if (migrationSessionStarted)
            {
                migrationSession = new MigrationSessionAsync(
                    migrationsToBeApplied,
                    GetFirstVersionOrDefault(migrationsToBeApplied),
                    GetLastVersionOrDefault(migrationsToBeApplied));
                await addMigrationSession(migrationSession).ConfigureAwait(false);
            }
            else
            {
                migrationSession = null;
            }

            return migrationSession;
        }
    }
}
