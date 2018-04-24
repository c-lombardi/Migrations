using Migrations.Types.ToBeImplemented;
using System.Collections.Generic;

namespace Migrations.ToBeImplemented
{
    public interface IMigrationLocator
    {
        /// <summary>
        /// Get all migrations that were and will be applied.
        /// </summary>
        /// <returns>The migrations that were and will be applied.</returns>
        IEnumerable<Migration> GetAllMigrations();
    }
}