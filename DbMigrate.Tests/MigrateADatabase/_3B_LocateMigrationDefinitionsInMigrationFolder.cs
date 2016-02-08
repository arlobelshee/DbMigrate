using DbMigrate.Model.Support;
using DbMigrate.Model.Support.Filesystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.MigrateADatabase
{
    [TestClass]
    public class _3B_LocateMigrationDefinitionsInMigrationFolder
    {
        private static DirectoryInMemory DirectoryWithTwoMigrations()
        {
            var folder = new DirectoryInMemory();
            folder.AddFile("1_first.migration.sql", _3B2_ExtractMigrationSpecFromFile.MigrationContentsForVersion(1));
            folder.AddFile("2_second.migration.sql", _3B2_ExtractMigrationSpecFromFile.MigrationContentsForVersion(2));
            return folder;
        }

        [TestMethod]
        public void FolderBasedMigrationLoaderShouldFindAllMigrationsInOneFolder()
        {
            var testSubject = new MigrationRepoDirectory(DirectoryWithTwoMigrations());
            testSubject.MaxMigrationVersionFound.Should().Be(2);
        }

        [TestMethod]
        public void FolderBasedMigrationLoaderShouldFindMigrationsThatAreDefinedInTheFolder()
        {
            var testSubject = new MigrationRepoDirectory(DirectoryWithTwoMigrations());
            var result = testSubject.LoadMigrationIfPresent(1);
            result.Should().NotBeNull();
            result.Version.Should().Be(1);
            result.Apply.Should().Be("create table Foo;");
        }

        [TestMethod]
        public void FolderBasedMigrationLoaderShouldFindNothingWithAnEmptyDirectory()
        {
            var testSubject = new MigrationRepoDirectory(new DirectoryInMemory());
            testSubject.MaxMigrationVersionFound.Should().Be(-1);
        }

        [TestMethod]
        public void FolderBasedMigrationLoaderShouldIgnoreNonMigrationFiles()
        {
            var repo = DirectoryWithTwoMigrations();
            repo.AddFile("readme.txt", "content is unimportant");
            var testSubject = new MigrationRepoDirectory(repo);
            testSubject.MaxMigrationVersionFound.Should().Be(2);
        }

        [TestMethod]
        public void FolderBasedMigrationLoaderShouldNotFindUndefinedMigrations()
        {
            var testSubject = new MigrationRepoDirectory(DirectoryWithTwoMigrations());
            var result = testSubject.LoadMigrationIfPresent(7);
            result.Should().BeNull();
        }
    }
}