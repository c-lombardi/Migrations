using System;

namespace Migrations.Types
{
    public class RepositoryMigration
    {
        internal readonly Migration Migration;
        public readonly MigrationVersion Version;
        public readonly string Description;
        public readonly DateTime StartedOn;
        public DateTime? CompletedOn { get; private set; }

        internal RepositoryMigration(Migration migration)
        {
            Migration = migration;
            StartedOn = DateTime.UtcNow;
            Version = migration.Version;
            Description = migration.Description;
        }

        internal void CompleteMigration()
        {
            CompletedOn = DateTime.UtcNow;
        }
    }
}