using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DbMigrate.Util;

namespace DbMigrate.Util
{
    public static class HelperMethods
    {
        public static IEnumerable<string> Lines(this TextReader source)
        {
            string nextLine;
            while (null != (nextLine = source.ReadLine()))
            {
                yield return nextLine;
            }
        }

        public static string RemoveSuffix(this string operand, string suffix)
        {
            return operand.Substring(0, operand.Length - suffix.Length);
        }

        public static string RemovePrefix(this string operand, string prefix)
        {
            return operand.Substring(prefix.Length);
        }

        public static string UpToFirst(this string operand, string delimiter)
        {
            var firstMatchPos = operand.IndexOf(delimiter);
            if (firstMatchPos < 0) return operand;
            return operand.Substring(0, firstMatchPos);
        }

        public static Task<T> ToTask<T>(this T returnValue)
        {
            return AsTask(() => returnValue);
        }

        public static Task<T> AsTask<T>(this Func<T> function)
        {
            var result = new Task<T>(function);
            result.Start();
            return result;
        }

        public static Task NoOpAction()
        {
            var result = new Task(() => { });
            result.Start();
            return result;
        }
    }
}

namespace System
{
    public static class SystemExtensions
    {
        public static string Format(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static void Each<T>(this IEnumerable<T> args, Action<T> op)
        {
            foreach (var a in args)
            {
                op(a);
            }
        }

        public static string StringJoin<T>(this IEnumerable<T> args, string separator)
        {
            return string.Join(separator, args);
        }

        public static FormatEnumerable<T> Format<T>(this IEnumerable<T> args, Func<T, string> itemFormat, string separator)
        {
            return new FormatEnumerable<T>(args, itemFormat, separator);
        }
    }
}