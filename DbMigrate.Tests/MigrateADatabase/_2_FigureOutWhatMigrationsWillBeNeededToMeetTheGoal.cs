using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Tests.__UtilitiesForTesting;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _2_FigureOutWhatMigrationsWillBeNeededToMeetTheGoal
	{
		private static MigrationSet RequestMigrationBetween(DatabaseVersion currentVersion, int? targetMin, int? targetMax,
			IMigrationLoader[] migrationsAvailable = null)
		{
			var available = migrationsAvailable ?? DefinedVersions(0);
			return ChangePlanner.MakePlan(null, new ChangeGoal(currentVersion, targetMin, targetMax),
				available);
		}

		public static IMigrationLoader[] DefinedVersions(params int[] versionNumbers)
		{
			return TestData.Migrations(versionNumbers).ToLoaders();
		}

		private static ChangePlan BeginUp(params int[] versions)
		{
			return new ChangePlan(Do.BeginUp, versions);
		}

		private static ChangePlan FinishUp(params int[] versions)
		{
			return new ChangePlan(Do.FinishUp, versions);
		}

		private static ChangePlan BeginDown(params int[] versions)
		{
			return new ChangePlan(Do.BeginDown, versions);
		}

		private static ChangePlan FinishDown(params int[] versions)
		{
			return new ChangePlan(Do.FinishDown, versions);
		}

		[Test]
		public void DownwardRangeShouldUnapplyCorrectMigrations()
		{
			var result = RequestMigrationBetween(new DatabaseVersion(2, 4), null, 2);
			result.Plan.Should().Be(BeginDown(4, 3));
		}

		[Test]
		public void RangeWithNoUpperBoundShouldGoToLatestMigrationFound()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4, 7);
			var result = RequestMigrationBetween(new DatabaseVersion(-1, -1), null, null, migrationsAvailable);
			result.Plan.Should().Be(BeginUp(0, 1, 2, 3, 4, 5, 6, 7));
		}

		[Test]
		public void RequestForNegativeVersionNumberShouldGoToThatManyFromTheEnd()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4);
			var result = RequestMigrationBetween(new DatabaseVersion(0, 0), null, -2, migrationsAvailable);
			result.Plan.Should().Be(BeginUp(1, 2));
		}

		[Test]
		public void RequestToGoToCurrentVersionShouldNoOp()
		{
			var result = RequestMigrationBetween(new DatabaseVersion(2, 3), null, 3);
			result.Plan.Should().Be(BeginUp());
		}

		[Test]
		public void RequestToUndoTopMigrationShouldDoSo()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4);
			var result = RequestMigrationBetween(new DatabaseVersion(2, 4), null, -2, migrationsAvailable);
			result.Plan.Should().Be(BeginDown(4, 3));
		}

		[Test]
		public void UnapplyTooManyMigrationsShouldAlwaysGoToVersionZero()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4);
			var result = RequestMigrationBetween(new DatabaseVersion(0, 4), null, -9, migrationsAvailable);
			result.Plan.Should().Be(BeginDown(4, 3, 2, 1));
		}

		[Test]
		public void UpwardRangeShouldBeAbleToStartWithDbHavingSomeMigrationsAlready()
		{
			var result = RequestMigrationBetween(new DatabaseVersion(1, 2), null, 4);
			result.Plan.Should().Be(BeginUp(3, 4));
		}

		[Test]
		public void UpwardRangeShouldIncludeBothEndpoints()
		{
			var result = RequestMigrationBetween(new DatabaseVersion(-1, -1), null, 3);
			result.Plan.Should().Be(BeginUp(0, 1, 2, 3));
		}
	}
}