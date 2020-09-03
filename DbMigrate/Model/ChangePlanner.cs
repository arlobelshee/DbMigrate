using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Filesystem;
using DbMigrate.UI;

namespace DbMigrate.Model
{
	public class ChangePlanner
	{
		private readonly IDatabase _database;
		private readonly ChangeGoal _request;

		public ChangePlanner(IDatabase database, ChangeGoal request)
		{
			_database = database;
			_request = request;
		}

		public MigrationSet UsingMigrationsFrom(string migrationFolder)
		{
			try
			{
				return MakePlan(_database, _request,
					new MigrationRepoMakeDbVersionAware(), new MigrationRepoDirectory(new DirectoryOnDisk(migrationFolder)));
			}
			catch (DirectoryNotFoundException)
			{
				throw new TerminateProgramWithMessageException(
					string.Format(UserMessage.ErrorMissingMigrationDirectory, migrationFolder),
					2);
			}
		}

		public static MigrationSet MakePlan(IDatabase database, ChangeGoal spec, params IMigrationLoader[] migrationLoaders)
		{
			var targetVersion = FindTrueTargetVersion(spec.TargetVersion, migrationLoaders);
			User.Notify(DescribePlan(spec.CurrentVersion, targetVersion));
			var plan = PlanToGetFromCurrentToTarget(spec, targetVersion);
			return new MigrationSet(plan, database, migrationLoaders);
		}

		private static ChangePlan PlanToGetFromCurrentToTarget(ChangeGoal spec, int targetVersion)
		{
			if (targetVersion >= spec.CurrentVersion)
			{
				var firstMigrationToApply = spec.CurrentVersion + 1;
				return new ChangePlan(Do.BeginUp,
					Enumerable.Range(firstMigrationToApply, targetVersion - firstMigrationToApply + 1));
			}
			else
			{
				var firstMigrationToApply = spec.CurrentVersion;
				return new ChangePlan(Do.BeginDown,
					Enumerable.Range(targetVersion + 1, firstMigrationToApply - targetVersion).Reverse());
			}
		}

		private static int FindTrueTargetVersion(int? targetVersionNumber, IEnumerable<IMigrationLoader> migrationLoaders)
		{
			var highestDefinedMigration = migrationLoaders.Max(l => l.MaxMigrationVersionFound);
			if (targetVersionNumber == null)
				return highestDefinedMigration;
			if (targetVersionNumber < 0)
				return Math.Max(0, highestDefinedMigration + targetVersionNumber.Value);
			return targetVersionNumber.Value;
		}

		private static string DescribePlan(int currentVersion, int targetVersion)
		{
			if (currentVersion == -1)
				return string.Format("Migrating version-unaware database to version {0}.", targetVersion);
			return "Migrating database from version {0} to version {1}.".Format(currentVersion, targetVersion);
		}
	}
}