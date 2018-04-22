using System;

namespace Migrations.Types
{
    public class RepositoryMigration
    {
        internal readonly Migration Migration;
        public MigrationVersion Version { get; set; }
        public string Description { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }

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