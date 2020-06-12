using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku.Ext
{
	public static class EnumerableExtender
	{
		public static void Each<T>(this IEnumerable<T> source, Action<T> iterator)
		{
			if (iterator == null)
				throw new NullReferenceException("iterator");
			foreach (T item in source)
				iterator(item);
		}
		public static IEnumerable<R> Each<T, R>(this IEnumerable<T> source, Func<T, R> iterator)
		{
			if (iterator == null)
				throw new NullReferenceException("iterator");
			foreach (T item in source)
				yield return iterator(item);
		}
	}
}
