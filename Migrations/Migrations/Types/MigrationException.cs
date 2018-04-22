using System;

namespace Migrations.Types
{
    public sealed class MigrationException : ApplicationException
    {
        public readonly MigrationVersion VersionFailedOn;

        public MigrationException(
            Exception innerException, 
            MigrationVersion versionFailedOn) : base(innerException.Message, innerException)
        {
            VersionFailedOn = versionFailedOn;
        }
    }
}