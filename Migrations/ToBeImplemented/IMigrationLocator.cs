using Migrations.Types;
using System.Collections.Generic;

namespace Migrations.ToBeImplemented
{
    public interface IMigrationLocator
    {
        IEnumerable<Migration> GetAllMigrations();
    }
}