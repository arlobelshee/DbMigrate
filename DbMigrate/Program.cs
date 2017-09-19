using System;
using System.Collections.Generic;
using Args;
using DbMigrate.Model;
using DbMigrate.UI;

namespace DbMigrate
{
	public class Program
	{
		private static int Main(string[] commandLine)
		{
			try
			{
				var args = ParseCommandLine(commandLine);
				User.OnNotify += Console.WriteLine;
				using (var db = new Target(args.ConnectionString, args.IsTestDatabase))
				{
					Console.WriteLine();
					db.MigrateTo(args.TargetVersion)
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
			try
			{
				var args = commandLineParser.CreateAndBind(commandLine);
				Require.Not(args.Help || string.IsNullOrEmpty(args.ConnectionString) ||
				            string.IsNullOrEmpty(args.Migrations));
				return args;
			}
			catch (Exception)
			{
				throw new TerminateAndShowHelp(commandLineParser);
			}
		}
	}
}