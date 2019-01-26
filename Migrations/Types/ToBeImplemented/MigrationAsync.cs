using Migrations.ToBeImplemented;
using System.Threading.Tasks;

namespace Migrations.Types.ToBeImplemented
{
    public abstract class MigrationAsync
    {
        public readonly MigrationVersion Version;
        public readonly string Description;

        protected MigrationAsync(
            MigrationVersion version,
            string description)
        {
            Version = version;
            Description = description;
        }

        public abstract Task UpdateAsync(IRepositoryAsync repository, IRepositoryToMigrate repositoryToMigrate);

        public abstract Task RollbackAsync(IRepositoryAsync repository, IRepositoryToMigrate repositoryToMigrate);
    }
}