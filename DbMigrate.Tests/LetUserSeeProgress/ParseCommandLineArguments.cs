using System;
using System.Linq;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.LetUserSeeProgress
{
	[TestFixture]
	public class ParseCommandLineArguments
	{
		[Test]
		public void EmptyCommandLineShouldRejectAndShowHelp()
		{
			CommandLine()
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void RequestingHelpShouldShowHelp()
		{
			CommandLine("--help")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void EmptyMigrationFolderShouldShowHelp()
		{
			CommandLine("--migrations", "--connectionstring", "valid_value", "--engine", "valid_value")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void MissingMigrationFolderShouldShowHelp()
		{
			CommandLine("--connectionstring", "valid_value", "--engine", "valid_value")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void EmptyConnectionStringShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--connectionstring", "--engine", "valid_value")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void MissingConnectionStringShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--engine", "valid_value")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void EmptyEngineShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--connectionstring", "valid_value", "--engine")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void MissingEngineShouldShowHelp()
		{
			CommandLine("--migrations", "valid_value", "--connectionstring", "valid_value")
				.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void ShouldFindMigrationAndConnectionString()
		{
			var parameters = ParseCommandLine("migration_folder", "the_connection_string", "the_engine");
			parameters.Migrations.Should().Be("migration_folder");
			parameters.ConnectionString.Should().Be("the_connection_string");
		}

		[Test]
		public void MissingTargetVersionShouldDefaultToNull()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "ignore");
			parameters.TargetVersion.Should().Be(null);
		}

		[Test]
		public void TargetVersionShouldBeReadIfPresent()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "ignore", "--targetversion", "3");
			parameters.TargetVersion.Should().Be(3);
		}

		[Test]
		public void NegativeTargetVersionShouldBeAllowed()
		{
			var parameters = ParseCommandLine("ignore", "ignore", "ignore", "--targetversion", "-9");
			parameters.TargetVersion.Should().Be(-9);
		}

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
	}
}