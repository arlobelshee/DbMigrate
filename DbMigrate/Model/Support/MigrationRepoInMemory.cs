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
			Migrations = migrations;
		}

		public IEnumerable<MigrationSpecification> Migrations { get; set; }

		public int MaxMigrationVersionFound
		{
			get { return Migrations.Max(m => m.Version); }
		}

		public MigrationSpecification LoadMigrationIfPresent(int version)
		{
			return Migrations.FirstOrDefault(m => m.Version == version);
		}
	}
}