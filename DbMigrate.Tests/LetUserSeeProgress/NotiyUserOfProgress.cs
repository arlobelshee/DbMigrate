using System.Collections.Generic;
using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using DbMigrate.Tests.__UtilitiesForTesting;
using DbMigrate.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.LetUserSeeProgress
{
    [TestClass]
    public class NotiyUserOfProgress
    {
        public TestContext TestContext { get; set; }

        [TestCleanup]
        public void CleanUp()
        {
            User.ClearNotify();
        }

        [TestInitialize]
        public void SetUp()
        {
            this._messagesSentToUser = new List<string>();
            User.OnNotify += this._messagesSentToUser.Add;
        }

        private List<string> _messagesSentToUser;

        private static void PlanToGoBetweenVersions(int currentVersion, int? targetVersion)
        {
            var databaseLocalMemory = new DatabaseLocalMemory();
            ChangePlanner.MakePlan(databaseLocalMemory, new ChangeGoal(currentVersion, targetVersion),
                TestData.Migrations(3, 4, 5).ToLoaders());
        }

        public static object[][] VersionPlanCases = new[]
            {
                new object[] {2, 4, "Migrating database from version 2 to version 4."},
                new object[] {9, null, "Migrating database from version 9 to version 5."},
                new object[] {9, -2, "Migrating database from version 9 to version 3."},
                new object[] {-1, 4, "Migrating version-unaware database to version 4."},
                new object[] {-1, null, "Migrating version-unaware database to version 5."},
                new object[] {-1, -2, "Migrating version-unaware database to version 3."},
            };

        [TestMethod]
        public void ApplyingMigrationShouldNotifyUser()
        {
            var testSubject = new ChangePlan(Do.Apply, new[] {3, 4});
            testSubject.ApplyTo(new DatabaseLocalMemory(), TestData.Migrations(3, 4).ToLoaders());

            this._messagesSentToUser.Should().ContainInOrder(new[]
                {
                    "Applying version 3 to the database.",
                    "Applying version 4 to the database."
                });
        }

        [TestMethod, TestCaseSource("VersionPlanCases")]
        public void ShouldExpressCorrectPlanToUser()
        {
            TestContext.Run((int currentVersion, int? targetVersion, string userMessage) =>
            {
                PlanToGoBetweenVersions(currentVersion, targetVersion);

                this._messagesSentToUser.Should().ContainInOrder(new[]
                {
                    userMessage,
                });
            });
        }

        [TestMethod]
        public void UnapplyingMigrationShouldNotifyUser()
        {
            var testSubject = new ChangePlan(Do.Unapply, new[] {5, 4});
            testSubject.ApplyTo(new DatabaseLocalMemory(), TestData.Migrations(4, 5).ToLoaders());

            this._messagesSentToUser.Should().ContainInOrder(new[]
                {
                    "Unapplying version 5 from the database.",
                    "Unapplying version 4 from the database.",
                });
        }
    }
}