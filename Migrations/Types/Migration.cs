using Migrations.ToBeImplemented;

namespace Migrations.Types
{
    public abstract class Migration
    {
        public MigrationVersion Version { get; protected set; }
        public abstract string Description { get; protected set; }

        protected Migration(
            MigrationVersion version,
            string description)
        {
            Version = version;
            Description = description;
        }

        public abstract void Update(IRepository repository);

        public abstract void Rollback(IRepository repository);
    }
}