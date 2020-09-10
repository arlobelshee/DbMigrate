using DbMigrate.Model;
using DbMigrate.Model.Support;
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
			database.SetMinVersionTo(31);
			database.SetMaxVersionTo(33);
			Target.FigureOutTheGoal(database, null, -9).Should().BeEquivalentTo(new {CurrentVersion = new DatabaseVersion(31, 33), TargetMin = null, TargetMax = -9},
				options => options.ExcludingMissingMembers());
		}
	}
}