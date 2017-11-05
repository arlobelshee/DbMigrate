using System;
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
			Action call = ()=> testSubject.Validate();
			call.ShouldThrow<TerminateAndShowHelp>();
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