using DbMigrate.Model.Support;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _3A_LocateMigrationToMakeDatabaseVersionAware
	{
		[Test]
		public void SpecialZeroLoaderShouldAlwaysFindTheOneMagicMigration()
		{
			var testSubject = new MigrationRepoMakeDbVersionAware();
			testSubject.MaxMigrationVersionFound.Should().Be(0);
			testSubject.LoadMigrationIfPresent(0).Should().NotBeNull();
		}
	}
}