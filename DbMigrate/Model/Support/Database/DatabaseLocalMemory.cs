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
			CurrentVersion = (-1).ToTask();
			AppliedMigrations = new List<MigrationSpecification>();
			UnappliedMigrations = new List<MigrationSpecification>();
		}

		public bool IsDisposed { get; private set; }
		public List<MigrationSpecification> AppliedMigrations { get; }
		public List<MigrationSpecification> UnappliedMigrations { get; }
		public bool CommittedTheChanges { get; private set; }
		public Task<int> CurrentVersion { get; private set; }
		public bool IsTestDatabase { get; set; }

		public void Dispose()
		{
			IsDisposed = true;
		}

		public void Commit()
		{
			if (AppliedMigrations.Count + UnappliedMigrations.Count > 0) CommittedTheChanges = true;
		}

		public Task SetCurrentVersionTo(int targetVersion)
		{
			CurrentVersion = targetVersion.ToTask();
			return HelperMethods.NoOpAction();
		}

		public void Apply(MigrationSpecification migration)
		{
			AppliedMigrations.Add(migration);
		}

		public void Unapply(MigrationSpecification migration)
		{
			UnappliedMigrations.Add(migration);
		}
	}
}