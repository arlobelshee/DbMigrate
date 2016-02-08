using System;
using DbMigrate.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;

namespace DbMigrate.Tests.LetUserSeeProgress
{
    [TestClass]
    public class ParseCommandLineArguments
    {
        [TestMethod]
        public void EmptyCommandLineShouldRejectAndShowHelp()
        {
            CommandLine()
                .ShouldThrow<TerminateAndShowHelp>();
        }

        [TestMethod]
        public void RequestingHelpShouldShowHelp()
        {
            CommandLine("--help")
                .ShouldThrow<TerminateAndShowHelp>();
        }

        [TestMethod]
        public void EmptyMigrationFolderShouldShowHelp()
        {
            CommandLine("--migrations", "--connectionstring", "valid_value")
                .ShouldThrow<TerminateAndShowHelp>();
        }

        [TestMethod]
        public void MissingMigrationFolderShouldShowHelp()
        {
            CommandLine("--connectionstring", "valid_value")
                .ShouldThrow<TerminateAndShowHelp>();
        }

        [TestMethod]
        public void EmptyConnectionStringShouldShowHelp()
        {
            CommandLine("--migrations", "asfed", "--connectionstring")
                .ShouldThrow<TerminateAndShowHelp>();
        }

        [TestMethod]
        public void MissingConnectionStringShouldShowHelp()
        {
            CommandLine("--migrations", "valid_value")
                .ShouldThrow<TerminateAndShowHelp>();
        }

        [TestMethod]
        public void ShouldFindMigrationAndConnectionString()
        {
            var parameters = ParseCommandLine("migration_folder", "the_connection_string");
            parameters.Migrations.Should().Be("migration_folder");
            parameters.ConnectionString.Should().Be("the_connection_string");
        }

        [TestMethod]
        public void MissingTargetVersionShouldDefaultToNull()
        {
            var parameters = ParseCommandLine("ignore", "ignore");
            parameters.TargetVersion.Should().Be(null);
        }

        [TestMethod]
        public void TargetVersionShouldBeReadIfPresent()
        {
            var parameters = ParseCommandLine("ignore", "ignore", "--targetversion", "3");
            parameters.TargetVersion.Should().Be(3);
        }

        [TestMethod]
        public void NegativeTargetVersionShouldBeAllowed()
        {
            var parameters = ParseCommandLine("ignore", "ignore", "--targetversion", "-9");
            parameters.TargetVersion.Should().Be(-9);
        }

        public static MigrationParameters ParseCommandLine(string migrationFolder, string connectionString, params string[] additionalArgs)
        {
            return Program.ParseCommandLine(
                new[] {"--migrations", migrationFolder,
                    "--connectionstring", connectionString}
                    .Concat(additionalArgs));
        }

        private static Action CommandLine(params string[] args)
        {
            return ()=> Program.ParseCommandLine(args);
        }
    }
}