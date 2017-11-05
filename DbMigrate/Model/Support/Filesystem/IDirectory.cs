using System;
using System.Collections.Generic;

namespace DbMigrate.Model.Support.Filesystem
{
	public interface IDirectory : IEquatable<IDirectory>
	{
		List<IFile> Files { get; }
	}
}