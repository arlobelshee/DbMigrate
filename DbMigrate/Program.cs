using System;
using System.Collections.Generic;
using Args;
using DbMigrate.Model;
using DbMigrate.Model.Support.Database;
using DbMigrate.UI;

namespace DbMigrate
{
	public class Program
	{
		private static int Main(string[] commandLine)
		{
			try
			{
				User.OnNotify += Console.WriteLine;
				var args = ParseCommandLine(commandLine);
				using (var db = new Target(args.ResolvedEngine, args.ConnectionString, args.IsTestDatabase))
				{
					Console.WriteLine();
					db.MigrateTo(args.TargetMin, args.TargetMax)
						.UsingMigrationsFrom(args.Migrations)
						.ExecuteAll();
				}
			}
			catch (TerminateProgramWithMessageException ex)
			{
				Console.Write(ex.Message);
				return ex.ErrorLevel;
			}
			catch (Exception ex)
			{
				Console.WriteLine();
				Console.WriteLine("!!");
				Console.WriteLine("!! Unexpected error encountered.");
				Console.WriteLine("!! Please report it so we can improve the migration tool.");
				Console.WriteLine("!!");
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine(ex);
				Console.WriteLine();
				return 100;
			}
			return 0;
		}

		public static MigrationParameters ParseCommandLine(IEnumerable<string> commandLine)
		{
			var commandLineParser = Configuration.Configure<MigrationParameters>();
			MigrationParameters args;
			try
			{
				args = commandLineParser.CreateAndBind(commandLine);
			}
			catch (Exception)
			{
				throw new TerminateAndShowHelp(commandLineParser);
			}
			args.Validate(commandLineParser);
			return args;
		}
	}
}