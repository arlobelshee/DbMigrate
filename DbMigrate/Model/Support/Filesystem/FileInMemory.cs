using System.IO;

namespace DbMigrate.Model.Support.Filesystem
{
	public class FileInMemory : IFile
	{
		private readonly string _contents;

		public FileInMemory(string fileName, string contents)
		{
			_contents = contents;
			Name = fileName;
		}

		public string Name { get; }

		public TextReader OpenText()
		{
			return new StringReader(_contents);
		}
	}
}