using System;

namespace Migrations.Types
{
    public sealed class MigrationException : Exception
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