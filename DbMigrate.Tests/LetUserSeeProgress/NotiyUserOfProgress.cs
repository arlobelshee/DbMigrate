using System.Collections.Generic;
using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Database;
using DbMigrate.Tests.__UtilitiesForTesting;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.LetUserSeeProgress
{
	[TestFixture]
	public class NotiyUserOfProgress
	{
		[TearDown]
		public void CleanUp()
		{
			User.ClearNotify();
		}

		[SetUp]
		public void SetUp()
		{
			_messagesSentToUser = new List<string>();
			User.OnNotify += _messagesSentToUser.Add;
		}

		public static object[][] VersionPlanCases =
		{
			new object[] {new DatabaseVersion(1, 2), null, 4, "Migrating database from version 2 to version 4."},
			new object[] {new DatabaseVersion(3, 9), null, null, "Migrating database from version 9 to version 5."},
			new object[] {new DatabaseVersion(3, 9), null, -2, "Migrating database from version 9 to version 3."},
			new object[] {new DatabaseVersion(-1, -1), null, 4, "Migrating version-unaware database to version 4."},
			new object[] { new DatabaseVersion(-1, -1), null, null, "Migrating version-unaware database to version 5."},
			new object[] { new DatabaseVersion(-1, -1), null, -2, "Migrating version-unaware database to version 3."}
		};

		private List<string> _messagesSentToUser;
		public TestContext TestContext { get; set; }

		private static void PlanToGoBetweenVersions(DatabaseVersion currentVersion, int? targetMin, int? targetMax)
		{
			var databaseLocalMemory = new DatabaseLocalMemory();
			ChangePlanner.MakePlan(databaseLocalMemory, new ChangeGoal(currentVersion, targetMin, targetMax),
				TestData.Migrations(3, 4, 5).ToLoaders());
		}

		[Test]
		public void ApplyingMigrationShouldNotifyUser()
		{
			var testSubject = new ChangePlan(Do.BeginUp, new[] {3, 4});
			testSubject.ApplyTo(new DatabaseLocalMemory(), TestData.Migrations(3, 4).ToLoaders());

			_messagesSentToUser.Should().ContainInOrder(new[]
			{
				"Applying version 3 to the database.",
				"Applying version 4 to the database."
			});
		}

		[Test]
		[TestCaseSource(nameof(VersionPlanCases))]
		public void ShouldExpressCorrectPlanToUser(DatabaseVersion currentVersion, int? targetMin, int? targetMax, string userMessage)
		{
			PlanToGoBetweenVersions(currentVersion, targetMin, targetMax);

			_messagesSentToUser.Should().ContainInOrder(new[]
			{
				userMessage
			});
		}

		[Test]
		public void UnapplyingMigrationShouldNotifyUser()
		{
			var testSubject = new ChangePlan(Do.BeginDown, new[] {5, 4});
			testSubject.ApplyTo(new DatabaseLocalMemory(), TestData.Migrations(4, 5).ToLoaders());

			_messagesSentToUser.Should().ContainInOrder(new[]
			{
				"Unapplying version 5 from the database.",
				"Unapplying version 4 from the database."
			});
		}
	}
}