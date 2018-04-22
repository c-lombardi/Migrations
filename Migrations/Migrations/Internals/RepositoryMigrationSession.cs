using System.Collections.Generic;
using System.Linq;
using Migrations.ToBeImplemented;
using Migrations.Types;

namespace Migrations.Internals
{
    internal class RepositoryMigrationSession
    {
        private static bool MigrationSessionIsExecuting(IRepository repository)
        {
            return repository.GetMigrationSessions()
                .Any(session => session.CompletedOn == null
                    && session.CompletedOnVersion == MigrationVersion.Default());
        }

        internal static void CompleteMigrationSession(
            IRepository repository, 
            MigrationSession migrationSession)
        {
            migrationSession.CompleteMigrationSession();
            repository.UpsertMigrationSession(migrationSession);
        }

        internal static void FailMigrationSession(
            IRepository repository, 
            MigrationSession migrationSession, 
            MigrationException migrationException)
        {
            migrationSession.FailMigrationSession(migrationException);
            repository.UpsertMigrationSession(migrationSession);
        }

        internal static bool TryStartMigrationSession(
            IRepository repository, 
            IEnumerable<Migration> migrationsToBeApplied, 
            out MigrationSession migrationSession)
        {
            migrationSession = MigrationSessionIsExecuting(repository)
                ? null
                : new MigrationSession(migrationsToBeApplied);

            return migrationSession != null;
        }
    }
}
