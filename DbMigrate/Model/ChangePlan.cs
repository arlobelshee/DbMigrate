using System;
using System.Collections.Generic;
using System.Linq;
using DbMigrate.Model.Support;
using DbMigrate.UI;

namespace DbMigrate.Model
{
	public class ChangePlan : IEquatable<ChangePlan>
	{
		public ChangePlan(Do operation, IEnumerable<int> migrationsInOrder)
		{
			Operation = operation;
			MigrationsInOrder = migrationsInOrder;
		}

		public Do Operation { get; }
		public IEnumerable<int> MigrationsInOrder { get; }

		public bool Equals(ChangePlan other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Operation, Operation) && other.MigrationsInOrder.SequenceEqual(MigrationsInOrder);
		}

		public void ApplyTo(IDatabase database, IMigrationLoader[] migrationLoaders)
		{
			if (MigrationsInOrder.Count() == 0) return;
			foreach (var version in MigrationsInOrder)
			{
				var migration = LoadMigrationForVersion(version, migrationLoaders);
				Require.Not(migration == null, 1, UserMessage.ErrorMissingMigration, Operation, version);
				User.Notify(Operation.UserMessageFor(version));
				Operation.Execute(database, migration);
			}
			Operation.SetResultingVersion(database, MigrationsInOrder.Last());
			database.Commit();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ChangePlan);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Operation.GetHashCode() * 397) ^ MigrationsInOrder.GetHashCode();
			}
		}

		public static bool operator ==(ChangePlan left, ChangePlan right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(ChangePlan left, ChangePlan right)
		{
			return !Equals(left, right);
		}

		public override string ToString()
		{
			return string.Format("{0} the migrations {1}.", Operation, MigrationsInOrder.StringJoin(", "));
		}

		private static MigrationSpecification LoadMigrationForVersion(int version,
			IEnumerable<IMigrationLoader> migrationLoaders)
		{
			return migrationLoaders
				.Select(loader => loader.LoadMigrationIfPresent(version))
				.FirstOrDefault(migration => migration != null);
		}
	}
}