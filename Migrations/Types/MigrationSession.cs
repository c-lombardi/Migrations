using Migrations.Types.ToBeImplemented;
using System;
using System.Collections.Generic;

namespace Migrations.Types
{
    public class MigrationSession
    {
        public MigrationSession() { }

        internal MigrationSession(
            IEnumerable<Migration> migrationsToBeApplied,
            MigrationVersion firstVersion,
            MigrationVersion lastVersion) : this()
        {
            MigrationSessionId = Guid.NewGuid();
            StartedOn = DateTime.UtcNow;
            MigrationsToBeApplied = migrationsToBeApplied;
            FirstVersion = firstVersion;
            LastVersion = lastVersion;
        }

        internal void CompleteMigrationSession()
        {
            CompletedSuccessfully = true;
            CompletedOn = DateTime.UtcNow;
            CompletedOnVersion = LastVersion;
        }

        internal void FailMigrationSession(MigrationException migrationException)
        {
            CompletedSuccessfully = false;
            CompletedOn = DateTime.UtcNow;
            CompletedOnVersion = migrationException.VersionFailedOn;
            InnerException = migrationException?.InnerException?.Message ?? string.Empty;
            StackTrace = migrationException?.InnerException?.StackTrace ?? string.Empty;
        }

        public Guid MigrationSessionId { get; set; }
        public MigrationVersion FirstVersion { get; set; }
        public MigrationVersion LastVersion { get; set; }
        public DateTime StartedOn { get; set; }
        public MigrationVersion CompletedOnVersion { get; set; }
        public DateTime? CompletedOn { get; set; }
        public bool CompletedSuccessfully { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }

        internal readonly IEnumerable<Migration> MigrationsToBeApplied;
    }
}
