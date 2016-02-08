using System.Collections.Generic;
using System.Linq;

namespace DbMigrate.Model.Support
{
    public class MigrationRepoInMemory : IMigrationLoader
    {
        public MigrationRepoInMemory() : this(new List<MigrationSpecification>())
        {
        }

        public MigrationRepoInMemory(List<MigrationSpecification> migrations)
        {
            this.Migrations = migrations;
        }

        public IEnumerable<MigrationSpecification> Migrations { get; set; }

        public int MaxMigrationVersionFound
        {
            get { return this.Migrations.Max(m => m.Version); }
        }

        public MigrationSpecification LoadMigrationIfPresent(int version)
        {
            return this.Migrations.FirstOrDefault(m => m.Version == version);
        }
    }
}