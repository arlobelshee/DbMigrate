using System;

namespace DbMigrate.Model.Support
{
	public abstract class Do
	{
		public static Do Apply = new _Apply();
		public static Do Unapply = new _Unapply();

		public abstract void Execute(IDatabase target, MigrationSpecification migration);
		public abstract void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied);
		public abstract string UserMessageFor(int version);

		private class _Apply : Do
		{
			public override void Execute(IDatabase target, MigrationSpecification migration)
			{
				target.Apply(migration);
			}

			public override void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied)
			{
				database.SetCurrentVersionTo(lastMigrationVersionApplied).Wait();
			}

			public override string UserMessageFor(int version)
			{
				return "Applying version {0} to the database.".Format(version);
			}

			public override string ToString()
			{
				return "Apply";
			}
		}

		private class _Unapply : Do
		{
			public override void Execute(IDatabase target, MigrationSpecification migration)
			{
				target.Unapply(migration);
			}

			public override void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied)
			{
				database.SetCurrentVersionTo(lastMigrationVersionApplied - 1).Wait();
			}

			public override string UserMessageFor(int version)
			{
				return "Unapplying version {0} from the database.".Format(version);
			}

			public override string ToString()
			{
				return "Unapply";
			}
		}
	}
}