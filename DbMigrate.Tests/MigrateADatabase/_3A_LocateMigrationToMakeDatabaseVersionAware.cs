using DbMigrate.Model.Support;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestClass]
	public class _3A_LocateMigrationToMakeDatabaseVersionAware
	{
		[TestMethod]
		public void SpecialZeroLoaderShouldAlwaysFindTheOneMagicMigration()
		{
			var testSubject = new MigrationRepoMakeDbVersionAware();
			testSubject.MaxMigrationVersionFound.Should().Be(0);
			testSubject.LoadMigrationIfPresent(0).Should().NotBeNull();
		}
	}
}