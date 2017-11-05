using System.IO;

namespace DbMigrate.Model.Support.Filesystem
{
	public class FileOnDisk : IFile
	{
		private readonly FileInfo _fileInfo;

		public FileOnDisk(FileInfo fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public string Name
		{
			get { return _fileInfo.Name; }
		}

		public TextReader OpenText()
		{
			return _fileInfo.OpenText();
		}
	}
}