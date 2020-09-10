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
			var targetVersion = FindTrueTargetVersion(spec.TargetMax, migrationLoaders);
			User.Notify(DescribePlan(spec.CurrentVersion, targetVersion));
			var plan = PlanToGetFromCurrentToTarget(spec, targetVersion);
			return new MigrationSet(plan, database, migrationLoaders);
		}

		private static ChangePlan PlanToGetFromCurrentToTarget(ChangeGoal spec, TargetVersion targetVersion)
		{
			if (targetVersion.IsAtLeast(spec.CurrentVersion))
			{
				var firstMigrationToApply = targetVersion.EndToMove.Pick(spec.CurrentVersion) + 1;
				return new ChangePlan(Do.BeginUp,
					Enumerable.Range((int)firstMigrationToApply, (int)(targetVersion.Destination - firstMigrationToApply + 1)));
			}
			else
			{
				var firstMigrationToApply = targetVersion.EndToMove.Pick(spec.CurrentVersion);
				return new ChangePlan(Do.BeginDown,
					Enumerable.Range((int)(targetVersion.Destination + 1), (int)(firstMigrationToApply - targetVersion.Destination)).Reverse());
			}
		}

		private static TargetVersion FindTrueTargetVersion(long? targetVersionNumber, IEnumerable<IMigrationLoader> migrationLoaders)
		{
			var highestDefinedMigration = migrationLoaders.Max(l => l.MaxMigrationVersionFound);
			if (targetVersionNumber == null)
				return new TargetVersion(TargetVersion.End.Max, highestDefinedMigration);
			if (targetVersionNumber < 0)
				return new TargetVersion(TargetVersion.End.Max,
					Math.Max(0, highestDefinedMigration + targetVersionNumber.Value));
			return new TargetVersion(TargetVersion.End.Max, targetVersionNumber.Value);
		}

		private static string DescribePlan(DatabaseVersion currentVersion, TargetVersion targetVersion)
		{
			if (currentVersion.Max == -1)
				return $"Migrating version-unaware database to version {targetVersion.Destination}.";
			return $"Migrating database {targetVersion.EndToMove} from version {targetVersion.EndToMove.Pick(currentVersion)} to version {targetVersion.Destination}.";
		}
	}
}