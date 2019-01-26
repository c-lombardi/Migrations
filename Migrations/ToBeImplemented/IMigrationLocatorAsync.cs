using Migrations.Types.ToBeImplemented;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Migrations.ToBeImplemented
{
    public interface IMigrationLocatorAsync
    {
        /// <summary>
        /// Get all migrations that were and will be applied.
        /// </summary>
        /// <returns>The migrations that were and will be applied.</returns>
        Task<IEnumerable<MigrationAsync>> GetAllMigrationsAsync();
    }
}