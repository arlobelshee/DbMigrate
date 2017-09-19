using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Tests.__UtilitiesForTesting;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestClass]
	public class _2_FigureOutWhatMigrationsWillBeNeededToMeetTheGoal
	{
		private static MigrationSet RequestMigrationBetween(int currentVersion, int? destinationVersion,
			IMigrationLoader[] migrationsAvailable = null)
		{
			var available = migrationsAvailable ?? DefinedVersions(0);
			return ChangePlanner.MakePlan(null, new ChangeGoal(currentVersion, destinationVersion),
				available);
		}

		public static IMigrationLoader[] DefinedVersions(params int[] versionNumbers)
		{
			return TestData.Migrations(versionNumbers).ToLoaders();
		}

		private static ChangePlan Apply(params int[] versions)
		{
			return new ChangePlan(Do.Apply, versions);
		}

		private static ChangePlan Unapply(params int[] versions)
		{
			return new ChangePlan(Do.Unapply, versions);
		}

		[TestMethod]
		public void DownwardRangeShouldUnapplyCorrectMigrations()
		{
			var result = RequestMigrationBetween(4, 2);
			result.Plan.Should().Be(Unapply(4, 3));
		}

		[TestMethod]
		public void RangeWithNoUpperBoundShouldGoToLatestMigrationFound()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4, 7);
			var result = RequestMigrationBetween(-1, null, migrationsAvailable);
			result.Plan.Should().Be(Apply(0, 1, 2, 3, 4, 5, 6, 7));
		}

		[TestMethod]
		public void RequestForNegativeVersionNumberShouldGoToThatManyFromTheEnd()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4);
			var result = RequestMigrationBetween(0, -2, migrationsAvailable);
			result.Plan.Should().Be(Apply(1, 2));
		}

		[TestMethod]
		public void RequestToGoToCurrentVersionShouldNoOp()
		{
			var result = RequestMigrationBetween(3, 3);
			result.Plan.Should().Be(Apply());
		}

		[TestMethod]
		public void RequestToUndoTopMigrationShouldDoSo()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4);
			var result = RequestMigrationBetween(4, -2, migrationsAvailable);
			result.Plan.Should().Be(Unapply(4, 3));
		}

		[TestMethod]
		public void UnapplyTooManyMigrationsShouldAlwaysGoToVersionZero()
		{
			var migrationsAvailable = DefinedVersions(0, 1, 2, 3, 4);
			var result = RequestMigrationBetween(4, -9, migrationsAvailable);
			result.Plan.Should().Be(Unapply(4, 3, 2, 1));
		}

		[TestMethod]
		public void UpwardRangeShouldBeAbleToStartWithDbHavingSomeMigrationsAlready()
		{
			var result = RequestMigrationBetween(2, 4);
			result.Plan.Should().Be(Apply(3, 4));
		}

		[TestMethod]
		public void UpwardRangeShouldIncludeBothEndpoints()
		{
			var result = RequestMigrationBetween(-1, 3);
			result.Plan.Should().Be(Apply(0, 1, 2, 3));
		}
	}
}