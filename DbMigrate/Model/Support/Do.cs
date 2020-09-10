using System;

namespace DbMigrate.Model.Support
{
	public abstract class Do
	{
		public static Do BeginUp = new _BeginUp();
		public static Do FinishUp = new _FinishUp();
		public static Do BeginDown = new _BeginDown();
		public static Do FinishDown = new _FinishDown();

		public abstract void Execute(IDatabase target, MigrationSpecification migration);
		public abstract void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied);
		public abstract string UserMessageFor(int version);

		private class _BeginUp : Do
		{
			public override void Execute(IDatabase target, MigrationSpecification migration)
			{
				target.BeginUpgrade(migration);
			}

			public override void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied)
			{
				database.SetMaxVersionTo(lastMigrationVersionApplied).Wait();
			}

			public override string UserMessageFor(int version)
			{
				return "Adding support for version {0} to the database.".Format(version);
			}

			public override string ToString()
			{
				return "Begin Upgrade";
			}
		}

		private class _FinishUp : Do
		{
			public override void Execute(IDatabase target, MigrationSpecification migration)
			{
				target.FinishUpgrade(migration);
			}

			public override void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied)
			{
				database.SetMinVersionTo(lastMigrationVersionApplied).Wait();
			}

			public override string UserMessageFor(int version)
			{
				return "Dropping support for old version {0} from the database.".Format(version-1);
			}

			public override string ToString()
			{
				return "Finish Upgrade";
			}
		}

		private class _BeginDown : Do
		{
			public override void Execute(IDatabase target, MigrationSpecification migration)
			{
				target.BeginDowngrade(migration);
			}

			public override void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied)
			{
				database.SetMinVersionTo(lastMigrationVersionApplied - 1).Wait();
			}

			public override string UserMessageFor(int version)
			{
				return "Adding support for old version {0} to the database.".Format(version - 1);
			}

			public override string ToString()
			{
				return "Begin Downgrade";
			}
		}

		private class _FinishDown : Do
		{
			public override void Execute(IDatabase target, MigrationSpecification migration)
			{
				target.FinishDowngrade(migration);
			}

			public override void SetResultingVersion(IDatabase database, int lastMigrationVersionApplied)
			{
				database.SetMaxVersionTo(lastMigrationVersionApplied - 1).Wait();
			}

			public override string UserMessageFor(int version)
			{
				return "Finishing rollback for version {0} to the database.".Format(version);
			}

			public override string ToString()
			{
				return "Begin Downgrade";
			}
		}
	}
}