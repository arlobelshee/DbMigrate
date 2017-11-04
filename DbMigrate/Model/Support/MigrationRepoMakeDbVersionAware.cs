using System;

namespace DbMigrate.Model.Support
{
	public class MigrationRepoMakeDbVersionAware : IMigrationLoader, IEquatable<MigrationRepoMakeDbVersionAware>
	{
		public bool Equals(MigrationRepoMakeDbVersionAware other)
		{
			return !ReferenceEquals(null, other);
		}

		public int MaxMigrationVersionFound => 0;

		public MigrationSpecification LoadMigrationIfPresent(int version)
		{
			if (version != 0)
				return null;
			return ToVersionZero();
		}

		private static MigrationSpecification ToVersionZero()
		{
			const string createVersionInfoTableSql =
				@"create table __database_info(
  version_number int
);
insert into __database_info(version_number) values(0);";
			const string dropVersionInfoTableSql = "drop table __database_info;";
			return new MigrationSpecification(0, "0_add_version_awareness", createVersionInfoTableSql, dropVersionInfoTableSql);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as MigrationRepoMakeDbVersionAware);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public static bool operator ==(MigrationRepoMakeDbVersionAware left, MigrationRepoMakeDbVersionAware right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(MigrationRepoMakeDbVersionAware left, MigrationRepoMakeDbVersionAware right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return "The migration to make a DB version-aware.";
		}
	}
}