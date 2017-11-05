using System;
using DbMigrate.Model.Support.Database;
using DbMigrate.UI;
using FluentAssertions;
using NUnit.Framework;

namespace DbMigrate.Tests.UnderstandWhatUserWantsToDo
{
	[TestFixture]
	public class UnderstandBsBuildTaskApiInvocation
	{
		[Test]
		public void EmptyRequestShouldBeRejectedAndPrintUsageInfo()
		{
			var testSubject = new MigrateTo();
			Action call = () => testSubject.Validate();
			call.ShouldThrow<TerminateAndShowHelp>();
		}

		[Test]
		public void InvalidEngineShouldBeRejectedWithUsefulErrorMessage()
		{
			var testSubject = new MigrateTo
			{
				Engine = "not a valid engine",
				ConnectionString = "valid",
				MigrationFolderName = "valid"
			};
			Action call = () => testSubject.Validate();
			call.ShouldThrow<TerminateProgramWithMessageException>()
				.WithMessage(
					$"I don't know the database engine '{testSubject.Engine}'. I only understand how to communicate with {DbEngine.KnownEngineNames}. Please extend me if you want to use that engine.");
		}

		[Test]
		public void ValidRequestShouldBeAccepted()
		{
			var testSubject = new MigrateTo {Engine = "sqlite", ConnectionString = "valid", MigrationFolderName = "valid"};
			Action call = () => testSubject.Validate();
			call.ShouldNotThrow();
		}
	}
}