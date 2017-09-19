using DbMigrate.Tests.LetUserSeeProgress;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.ApplyTestDataToTestSytems
{
	[TestClass]
	public class CommandLineAllowsUserToSpecifyWhetherADatabaseIsATestDatabase
	{
		[TestMethod]
		public void IsTestDatabaseShouldDefaultToFalse()
		{
			var parameters = ParseCommandLineArguments.ParseCommandLine("ignore", "ignore");
			parameters.IsTestDatabase.Should().BeFalse();
		}

		[TestMethod]
		public void ShouldNoticeThisIsATestDatabaseWhenSwitchIsPresent()
		{
			var parameters = ParseCommandLineArguments.ParseCommandLine("ignore", "ignore", "--istestdatabase");
			parameters.IsTestDatabase.Should().BeTrue();
		}
	}
}