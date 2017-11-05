using System;
using System.Runtime.Serialization;

namespace DbMigrate.UI
{
	public class TerminateProgramWithMessageException : Exception
	{
		public TerminateProgramWithMessageException(string message, int errorLevel) : base(message)
		{
			ErrorLevel = errorLevel;
		}

		protected TerminateProgramWithMessageException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public int ErrorLevel { get; }
	}
}