using DbMigrate.Tests.LetUserSeeProgress;
using DbMigrate.Tests.UnderstandWhatUserWantsToDo;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.ApplyTestDataToTestSytems
{
	[TestFixture]
	public class CommandLineAllowsUserToSpecifyWhetherADatabaseIsATestDatabase
	{
		[Test]
		public void IsTestDatabaseShouldDefaultToFalse()
		{
			var parameters = ParseCommandLineArguments.ParseCommandLine("ignore", "ignore", "ignore");
			parameters.IsTestDatabase.Should().BeFalse();
		}

		[Test]
		public void ShouldNoticeThisIsATestDatabaseWhenSwitchIsPresent()
		{
			var parameters = ParseCommandLineArguments.ParseCommandLine("ignore", "ignore", "ignore", "--istestdatabase");
			parameters.IsTestDatabase.Should().BeTrue();
		}
	}
}