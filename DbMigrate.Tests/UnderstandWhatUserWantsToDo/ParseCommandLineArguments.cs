using System;
using System.Linq;
using DbMigrate.Model.Support.Database;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.UnderstandWhatUserWantsToDo
{
	[TestFixture]
	public class ParseCommandLineArguments
	{
		public static MigrationParameters ParseCommandLine(string migrationFolder, string connectionString, string engine,
			params string[] additionalArgs)
		{
			return Program.ParseCommandLine(
				new[]
					{
						"--migrations", migrationFolder,
						"--connectionstring", connectionString,
						"--engine", engine
					}
					.Concat(additionalArgs));
		}

		private static Action CommandLine(params string[] args)
		{
			return () => Program.ParseCommandLine(args);
		}

		[Test]
		public void EmptyCommandLineShouldRejectAndShowHelp()
		{
			CommandLine()
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void EmptyConnectionStringShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--connectionstring", "--engine", "valid_value")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void EmptyEngineShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--connectionstring", "valid_value", "--engine")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void EmptyMigrationFolderShouldShowHelp()
		{
			CommandLine("--migrations", "--connectionstring", "valid_value", "--engine", "valid_value")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void ChangingBothVersionsShouldShowError()
		{
			CommandLine("--migrations", "ignore", "--connectionstring", "ignore", "--engine", "sqlite", "--targetmin", "3", "--targetmax", "9")
				.Should().Throw<TerminateProgramWithMessageException>().WithMessage(@"Cannot update both the min and max version at once.

Please update one end of the range at a time.");
		}

		[Test]
		public void MissingConnectionStringShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--engine", "valid_value")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void MissingEngineShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--connectionstring", "valid_value")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void MissingMigrationFolderShouldShowHelp()
		{
			CommandLine("--connectionstring", "valid_value", "--engine", "valid_value")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void MissingTargetVersionShouldDefaultToNull()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "sqlite");
			parameters.TargetMin.Should().Be(null);
			parameters.TargetMax.Should().Be(null);
		}

		[Test]
		public void NegativeTargetVersionShouldBeAllowed()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "sqlite", "--targetmax", "-9");
			parameters.TargetMax.Should().Be(-9);
		}

		[Test]
		public void RequestingHelpShouldShowHelp()
		{
			CommandLine("--help")
				.Should().Throw<TerminateAndShowHelp>();
		}

		[Test]
		public void ShouldFindAllRequiredParams()
		{
			var parameters = ParseCommandLine("migration_folder", "the_connection_string", "sqlite");
			parameters.Migrations.Should().Be("migration_folder");
			parameters.ConnectionString.Should().Be("the_connection_string");
			parameters.Engine.Should().Be("sqlite");
			parameters.ResolvedEngine.Should().Be(DbEngine.SqlLite);
		}

		[Test]
		public void TargetMaxShouldBeReadIfPresent()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "sqlite", "--targetmax", "3");
			parameters.TargetMax.Should().Be(3);
		}

		[Test]
		public void TargetMinShouldBeReadIfPresent()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "sqlite", "--targetmin", "3");
			parameters.TargetMin.Should().Be(3);
		}
	}
}