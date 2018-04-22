using System;
using System.Collections.Generic;
using System.Linq;

namespace Migrations.Types
{
    public class MigrationSession
    {
        private MigrationSession()
        {
            MigrationSessionId = Guid.NewGuid();
            StartedOn = DateTime.UtcNow;
        }

        internal MigrationSession(IEnumerable<Migration> migrations) : this()
        {
            MigrationsToBeApplied = migrations;
            if (migrations.Any())
            {
                FirstVersion = migrations.Min(migration => migration.Version);
                LastVersion = migrations.Max(migration => migration.Version);
            }
            else
            {
                FirstVersion = MigrationVersion.Default();
                LastVersion = MigrationVersion.Default();
            }
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

        private static string _getSuccessfulString(bool completedSuccessfully)
        {
            return completedSuccessfully
                ? "successful"
                : "unsuccessful";
        }
    }
}
