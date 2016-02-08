using System;
using System.Collections.Generic;
using System.Linq;
using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using DbMigrate.Tests.__UtilitiesForTesting;
using DbMigrate.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.MigrateADatabase
{
    [TestClass]
    public class _4_ImplementMigrationPlan
    {
        [TestInitialize]
        public void Setup()
        {
            this._database = new DatabaseLocalMemory();
            this._definedMigrations = TestData.Migrations(1, 2, 3, 4);
        }

        private DatabaseLocalMemory _database;
        private MigrationSpecification[] _definedMigrations;

        private static IEnumerable<MigrationSpecification> MigrationsForVersions(
            IEnumerable<MigrationSpecification> migrations,
            params int[] versionNumbers)
        {
            return versionNumbers.Select(n => migrations.First(m => m.Version == n));
        }

        [TestMethod]
        public void ApplyingNoMigrationsShouldDoNothing()
        {
            var testSubject = new ChangePlan(Do.Apply, new int[] {});
            testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            this._database.AppliedMigrations.Should().BeEmpty();
            this._database.CommittedTheChanges.Should().BeFalse();
        }

        [TestMethod]
        public void PlanToApplyShouldLoadCorrectMigrationsAndApplyThemAgainstTheDatabase()
        {
            var testSubject = new ChangePlan(Do.Apply, new[] {2, 3});
            testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            this._database.AppliedMigrations.Should().ContainInOrder(MigrationsForVersions(this._definedMigrations, 2, 3));
        }

        [TestMethod]
        public void PlanToApplyShouldSetDatabaseVersionToLastMigrationApplied()
        {
            var testSubject = new ChangePlan(Do.Apply, new[] {2, 3});
            testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            this._database.CurrentVersion.Result.Should().Be(3);
        }

        [TestMethod]
        public void PlanToUnapplyShouldLoadCorrectMigrationsAndUnapplyThemAgainstTheDatabase()
        {
            var testSubject = new ChangePlan(Do.Unapply, new[] {4, 3});
            testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            this._database.UnappliedMigrations.Should().ContainInOrder(MigrationsForVersions(this._definedMigrations, 4, 3));
        }

        [TestMethod]
        public void PlanToUnapplyShouldSetDatabaseVersionToOneLessThanLastMigrationUnapplied()
        {
            var testSubject = new ChangePlan(Do.Unapply, new[] {4, 3});
            testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            this._database.CurrentVersion.Result.Should().Be(2);
        }

        [TestMethod]
        public void ShouldCommitAllChangesToTheDatabase()
        {
            var testSubject = new ChangePlan(Do.Unapply, new[] {4, 3});
            testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            this._database.CommittedTheChanges.Should().BeTrue();
        }

        [TestMethod]
        public void ShouldGiveGoodErrorWhenAttemptToApplyUndefinedMigration()
        {
            var testSubject = new ChangePlan(Do.Unapply, new[] {19});
            Action application = () => testSubject.ApplyTo(this._database, this._definedMigrations.ToLoaders());
            application.ShouldThrow<TerminateProgramWithMessageException>().WithMessage(
                @"Missing migration 19

I needed to Unapply migration 19, but could not find a definition for it.
Please make sure there is a file in your migration directory named

'19_some_name.migration.sql'."
                ).And.ErrorLevel.Should().Be(1);
        }
    }
}