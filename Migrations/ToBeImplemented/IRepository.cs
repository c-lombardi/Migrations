using Migrations.Types;
using System.Collections.Generic;

namespace Migrations.ToBeImplemented
{
    public interface IRepository
    {
        IEnumerable<MigrationSession> GetMigrationSessions();
        IEnumerable<RepositoryMigration> GetMigrations();

        void AddMigrationSession(MigrationSession migrationSession);
        void AddMigration(RepositoryMigration migration);

        void UpsertMigrationSession(MigrationSession migration);
        void UpsertMigration(RepositoryMigration migration);

        void DeleteMigration(RepositoryMigration migration);
    }
}
