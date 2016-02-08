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
            this._items = items;
            this._itemFormat = itemFormat;
            this._separator = separator;
        }

        public override string ToString()
        {
            return this._items.Select(this._itemFormat).StringJoin(this._separator);
        }
    }
}