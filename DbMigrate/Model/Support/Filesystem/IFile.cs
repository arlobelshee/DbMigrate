using System.IO;

namespace DbMigrate.Model.Support.Filesystem
{
	public interface IFile
	{
		string Name { get; }
		TextReader OpenText();
	}
}