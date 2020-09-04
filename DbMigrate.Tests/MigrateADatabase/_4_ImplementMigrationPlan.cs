using System;
using System.Collections.Generic;
using System.Linq;
using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using DbMigrate.Tests.__UtilitiesForTesting;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _4_ImplementMigrationPlan
	{
		private DatabaseLocalMemory _database;
		private MigrationSpecification[] _definedMigrations;

		[SetUp]
		public void Setup()
		{
			_database = new DatabaseLocalMemory();
			_definedMigrations = TestData.Migrations(1, 2, 3, 4);
		}

		private static IEnumerable<MigrationSpecification> MigrationsForVersions(
			IEnumerable<MigrationSpecification> migrations,
			params int[] versionNumbers)
		{
			return versionNumbers.Select(n => migrations.First(m => m.Version == n));
		}

		[Test]
		public void ApplyingNoMigrationsShouldDoNothing()
		{
			var testSubject = new ChangePlan(Do.BeginUp, new int[] { });
			testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			_database.AppliedMigrations.Should().BeEmpty();
			_database.CommittedTheChanges.Should().BeFalse();
		}

		[Test]
		public void PlanToApplyShouldLoadCorrectMigrationsAndApplyThemAgainstTheDatabase()
		{
			var testSubject = new ChangePlan(Do.BeginUp, new[] {2, 3});
			testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			_database.AppliedMigrations.Should().ContainInOrder(MigrationsForVersions(_definedMigrations, 2, 3));
		}

		[Test]
		public void PlanToApplyShouldSetDatabaseVersionToLastMigrationApplied()
		{
			var testSubject = new ChangePlan(Do.BeginUp, new[] {2, 3});
			testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			_database.MaxVersion.Result.Should().Be(3);
		}

		[Test]
		public void PlanToUnapplyShouldLoadCorrectMigrationsAndUnapplyThemAgainstTheDatabase()
		{
			var testSubject = new ChangePlan(Do.BeginDown, new[] {4, 3});
			testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			_database.UnappliedMigrations.Should().ContainInOrder(MigrationsForVersions(_definedMigrations, 4, 3));
		}

		[Test]
		public void PlanToUnapplyShouldSetDatabaseVersionToOneLessThanLastMigrationUnapplied()
		{
			var testSubject = new ChangePlan(Do.BeginDown, new[] {4, 3});
			testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			_database.MaxVersion.Result.Should().Be(2);
		}

		[Test]
		public void ShouldCommitAllChangesToTheDatabase()
		{
			var testSubject = new ChangePlan(Do.BeginDown, new[] {4, 3});
			testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			_database.CommittedTheChanges.Should().BeTrue();
		}

		[Test]
		public void ShouldGiveGoodErrorWhenAttemptToApplyUndefinedMigration()
		{
			var testSubject = new ChangePlan(Do.BeginDown, new[] {19});
			Action application = () => testSubject.ApplyTo(_database, _definedMigrations.ToLoaders());
			application.Should().Throw<TerminateProgramWithMessageException>().WithMessage(
				@"Missing migration 19

I needed to Unapply migration 19, but could not find a definition for it.
Please make sure there is a file in your migration directory named

'19_some_name.migration.sql'."
			).And.ErrorLevel.Should().Be(1);
		}
	}
}