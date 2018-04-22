using System;

namespace Migrations.Types
{
    public class RepositoryMigration
    {
        internal readonly Migration Migration;
        public MigrationVersion Version { get; protected set; }
        public string Description { get; protected set; }
        public DateTime StartedOn { get; protected set; }
        public DateTime? CompletedOn { get; protected set; }

        public RepositoryMigration(Migration migration)
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