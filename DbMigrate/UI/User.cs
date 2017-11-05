using System;

namespace DbMigrate.UI
{
	public static class User
	{
		public static void ClearNotify()
		{
			OnNotify = null;
		}

		public static event Action<string> OnNotify;

		public static void Notify(string message)
		{
			if (null != OnNotify)
				OnNotify(message);
		}
	}
}