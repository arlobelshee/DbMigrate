using System;
using System.IO;
using DbMigrate.Model;
using DbMigrate.Model.Support.FileFormat;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrate.Tests.MigrateADatabase
{
    [TestClass]
    public class _3B2_ExtractMigrationSpecFromFile
    {
        private static readonly string TrivialMigration = MigrationContentsForVersion(3345);
        private const string ValidFileName = "3345_some_migration_name.migration.sql";

        public static string MigrationContentsForVersion(int version)
        {
            return
                String.Format(
                    @"
-- Migration version: {0}
-- Full-line comments outside of sections should be ignored

-- Migration apply --

-- Full-line comments in sections should be ignored too.
create table Foo;

-- Migration insert test data --

insert into Foo;

-- Migration delete test data --

delete from Foo;

-- Migration unapply --

drop table Foo;
",
                    version);
        }

        [TestMethod]
        public void LoadingFileShouldFindDowngradeScript()
        {
            var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
            testSubject.Unapply.Should().Be("drop table Foo;");
        }

        [TestMethod]
        public void LoadingFileShouldFindMigrationVersionNumber()
        {
            var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
            testSubject.Version.Should().Be(3345);
        }

        [TestMethod]
        public void LoadingFileShouldFindSectionToAddTestData()
        {
            var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
            testSubject.InsertTestData.Should().Be("insert into Foo;");
        }

        [TestMethod]
        public void LoadingFileShouldFindSectionToDeleteTestData()
        {
            var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
            testSubject.DeleteTestData.Should().Be("delete from Foo;");
        }

        [TestMethod]
        public void LoadingFileShouldFindUpgradeScript()
        {
            var testSubject = new MigrationSpecification(new MigrationFile(new StringReader(TrivialMigration), ValidFileName));
            testSubject.Apply.Should().Be("create table Foo;");
        }

        [TestMethod]
        public void ShouldKnowHowToGetVersionNumberFromFileName()
        {
            MigrationFile.FileNameVersion("3_asfasdf.migration.sql").Should().Be(3);
        }
    }
}