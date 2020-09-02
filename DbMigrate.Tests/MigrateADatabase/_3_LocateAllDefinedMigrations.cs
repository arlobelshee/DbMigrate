using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Filesystem;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.MigrateADatabase
{
	[TestFixture]
	public class _3_LocateAllDefinedMigrations
	{
		[Test]
		public void MigrationSetShouldFindMigrationsFromDiskAndSpecialZeroMigrationLoader()
		{
			const string dirName = "c:\\";
			var testSubject = new ChangePlanner(null, new ChangeGoal(0, null));
			var result = testSubject.UsingMigrationsFrom(dirName);
			result.Loaders.Should().BeEquivalentTo(
				new MigrationRepoDirectory(new DirectoryOnDisk(dirName)),
				new MigrationRepoMakeDbVersionAware());
		}

		[Test]
		public void MigrationSetShouldGiveGoodErrorWhenGivenInvalidDirectory()
		{
			const string dirName = @"c:\directory\that\does\not\exist";
			var testSubject = new ChangePlanner(null, new ChangeGoal(0, null));
			testSubject.Invoking(t => t.UsingMigrationsFrom(dirName))
				.Should().Throw<TerminateProgramWithMessageException>()
				.WithMessage(
					@"Could not find migration directory.

You said migrations were in 'c:\directory\that\does\not\exist'.
However, I could not find that directory.")
				.And.ErrorLevel.Should().Be(2);
		}
	}
}