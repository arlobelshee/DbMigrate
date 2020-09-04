using System.Collections.Generic;
using System.Threading.Tasks;
using DbMigrate.Util;

namespace DbMigrate.Model.Support.Database
{
	public class DatabaseLocalMemory : IDatabase
	{
		public DatabaseLocalMemory()
		{
			IsDisposed = false;
			MaxVersion = (-1).ToTask();
			AppliedMigrations = new List<MigrationSpecification>();
			UnappliedMigrations = new List<MigrationSpecification>();
		}

		public bool IsDisposed { get; private set; }
		public List<MigrationSpecification> AppliedMigrations { get; }
		public List<MigrationSpecification> UnappliedMigrations { get; }
		public bool CommittedTheChanges { get; private set; }
		public Task<int> MaxVersion { get; private set; }
		public bool IsTestDatabase { get; set; }

		public void Dispose()
		{
			IsDisposed = true;
		}

		public void Commit()
		{
			if (AppliedMigrations.Count + UnappliedMigrations.Count > 0) CommittedTheChanges = true;
		}

		public Task SetMaxVersionTo(int targetVersion)
		{
			MaxVersion = targetVersion.ToTask();
			return HelperMethods.NoOpAction();
		}

		public void BeginUpgrade(MigrationSpecification migration)
		{
			AppliedMigrations.Add(migration);
		}

		public void BeginDowngrade(MigrationSpecification migration)
		{
			UnappliedMigrations.Add(migration);
		}
	}
}