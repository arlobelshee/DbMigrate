using System;
using System.Collections.Generic;
using System.Linq;

namespace DbMigrate.Util
{
	public class FormatEnumerable<T>
	{
		private readonly Func<T, string> _itemFormat;
		private readonly IEnumerable<T> _items;
		private readonly string _separator;

		public FormatEnumerable(IEnumerable<T> items, Func<T, string> itemFormat, string separator)
		{
			_items = items;
			_itemFormat = itemFormat;
			_separator = separator;
		}

		public override string ToString()
		{
			return _items.Select(_itemFormat).StringJoin(_separator);
		}
	}
}