using DbMigrate.Model;
using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Filesystem;
using DbMigrate.UI;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.MigrateADatabase
{
    [TestClass]
    public class _3_LocateAllDefinedMigrations
    {
        [TestMethod]
        public void MigrationSetShouldFindMigrationsFromDiskAndSpecialZeroMigrationLoader()
        {
            const string dirName = "c:\\";
            var testSubject = new ChangePlanner(null, new ChangeGoal(0, null));
            var result = testSubject.UsingMigrationsFrom(dirName);
            result.Loaders.Should().BeEquivalentTo(
                new MigrationRepoDirectory(new DirectoryOnDisk(dirName)),
                new MigrationRepoMakeDbVersionAware());
        }

        [TestMethod]
        public void MigrationSetShouldGiveGoodErrorWhenGivenInvalidDirectory()
        {
            const string dirName = @"c:\directory\that\does\not\exist";
            var testSubject = new ChangePlanner(null, new ChangeGoal(0, null));
            testSubject.Invoking(t => t.UsingMigrationsFrom(dirName))
                .ShouldThrow<TerminateProgramWithMessageException>()
                .WithMessage(
                    @"Could not find migration directory.

You said migrations were in 'c:\directory\that\does\not\exist'.
However, I could not find that directory.")
                .And.ErrorLevel.Should().Be(1);
        }
    }
}