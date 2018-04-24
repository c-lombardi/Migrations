using Migrations.Types;
using Migrations.Types.ToBeImplemented;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Migrations.Internals
{
    internal static class MigrationSessionHelpers
    {
        internal static bool MigrationSessionIsAvailableToExecute(IEnumerable<MigrationSession> migrationSessions)
        {
            return !migrationSessions
                .Any(session => session.CompletedOn == null
                    && session.CompletedOnVersion == MigrationVersion.Default());
        }

        internal static void CompleteMigrationSession(
            Action<MigrationSession> upsertMigrationSession,
            MigrationSession migrationSession)
        {
            migrationSession.CompleteMigrationSession();
            upsertMigrationSession(migrationSession);
        }

        internal static void FailMigrationSession(
            Action<MigrationSession> upsertMigrationSession,
            MigrationSession migrationSession,
            MigrationException migrationException)
        {
            migrationSession.FailMigrationSession(migrationException);
            upsertMigrationSession(migrationSession);
        }

        internal static MigrationVersion GetLastVersionOrDefault(IEnumerable<Migration> migrationsToBeApplied)
        {
            return migrationsToBeApplied.Any()
                ? migrationsToBeApplied.Max(migration => migration.Version)
                : MigrationVersion.Default();
        }

        internal static MigrationVersion GetFirstVersionOrDefault(IEnumerable<Migration> migrationsToBeApplied)
        {
            return migrationsToBeApplied.Any()
                ? migrationsToBeApplied.Min(migration => migration.Version)
                : MigrationVersion.Default();
        }

        internal static bool TryStartMigrationSession(
            Action<MigrationSession> addMigrationSession,
            bool migrationSessionStarted,
            IEnumerable<Migration> migrationsToBeApplied,
            out MigrationSession migrationSession)
        {
            if (migrationSessionStarted)
            {
                migrationSession = new MigrationSession(
                    migrationsToBeApplied,
                    GetFirstVersionOrDefault(migrationsToBeApplied),
                    GetLastVersionOrDefault(migrationsToBeApplied));
                addMigrationSession(migrationSession);
            }
            else
            {
                migrationSession = null;
            }

            return migrationSessionStarted;
        }
    }
}
