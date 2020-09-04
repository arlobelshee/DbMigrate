using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _1_PickTheGoal
	{
		[Test]
		public void ShouldChooseToGoFromDatabaseCurrentVersionToTarget()
		{
			var database = new DatabaseLocalMemory();
			database.SetMaxVersionTo(33);
			Target.FigureOutTheGoal(database, -9).Should().BeEquivalentTo(new {CurrentVersion = 33, TargetVersion = -9},
				options => options.ExcludingMissingMembers());
		}
	}
}