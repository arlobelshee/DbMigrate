using System;

namespace DbMigrate.UI
{
	internal static class Require
	{
		public static void That(bool condition)
		{
			if (!condition)
				throw new Exception("Prereq failed");
		}

		public static void That(bool condition, int errorlevel, string messageForUser, params object[] messageArgs)
		{
			if (!condition)
				throw new TerminateProgramWithMessageException(messageForUser.Format(messageArgs), errorlevel);
		}

		public static void Not(bool condition)
		{
			That(!condition);
		}

		public static void Not(bool condition, int errorlevel, string messageForUser, params object[] messageArgs)
		{
			That(!condition, errorlevel, messageForUser, messageArgs);
		}
	}
}