using Migrations.ToBeImplemented;

namespace Migrations.Types
{
    public abstract class Migration
    {
        public MigrationVersion Version { get; protected set; }
        public abstract string Description { get; protected set; }
        public abstract IRepository Repository { get; protected set; }

        protected Migration(
            MigrationVersion version, 
            IRepository repository, 
            string description)
        {
            Version = version;
            Repository = repository;
            Description = description;
        }

        public abstract void Update();

        public abstract void Rollback();
    }
}