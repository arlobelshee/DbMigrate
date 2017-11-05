using System;
using System.Text;

namespace DbMigrate.Model.Support.FileFormat
{
	internal class SectionFinder
	{
		private readonly StringBuilder _buffer = new StringBuilder();
		private readonly Action<string> _whereToStoreResults;
		private bool _hasBeenEncountered;

		public SectionFinder(int sectionOrder, string name, Action<string> whereToStoreResults)
		{
			Order = sectionOrder;
			Name = name;
			_whereToStoreResults = whereToStoreResults;
		}

		public string Name { get; }

		public int Order { get; }

		public bool IsEmpty => !_hasBeenEncountered;

		public void OnBeginSection()
		{
			_hasBeenEncountered = true;
		}

		public void Store()
		{
			_whereToStoreResults(_buffer.ToString().Trim());
		}

		public bool IsFor(string sectionName)
		{
			return Name == sectionName;
		}

		public void AppendLine(string line)
		{
			_buffer.AppendLine(line);
		}
	}
}