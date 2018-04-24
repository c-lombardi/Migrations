using Migrations.ToBeImplemented;

namespace Migrations.Types.ToBeImplemented
{
    public abstract class Migration
    {
        public readonly MigrationVersion Version;
        public readonly string Description;

        protected Migration(
            MigrationVersion version,
            string description)
        {
            Version = version;
            Description = description;
        }

        public abstract void Update(IRepository repository, IRepositoryToMigrate repositoryToMigrate);

        public abstract void Rollback(IRepository repository, IRepositoryToMigrate repositoryToMigrate);
    }
}