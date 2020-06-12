using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Ext;

namespace Sudoku.Model
{
	public class Nine : ICollection<Cell>, IList<Cell>
	{
		private List<Cell> list = new List<Cell>(9);

		public string Attribute { get; set; }

		public void Add(Cell item)
		{
			if (list.Count == 9)
				throw new InvalidOperationException();
			list.Add(item);
			item.OnValueChanged += item_OnValueChanged;
		}

		public event Action<Nine> OnValidationError;

		void item_OnValueChanged(int oldValue, int newValue)
		{
			if (!IsValid())
				if (OnValidationError != null)
					OnValidationError(this);
		}

		public bool IsValid()
		{
			var numbers = (from Cell cell in list
								where cell.Value > 0
								select cell.Value).ToArray();
			return numbers.SequenceEqual(numbers.Distinct());
		}

		public override string ToString()
		{
			if (list.Count != 9)
				return "Incomplete";
			return string.Join(" ", list.Each<Cell, string>(cell => cell.ToString()).ToArray());
		}


		public void Clear()
		{
			list.Clear();
		}

		public bool Contains(Cell item)
		{
			return list.Contains(item);
		}

		public void CopyTo(Cell[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(Cell item)
		{
			return list.Remove(item);
		}

		public IEnumerator<Cell> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)list).GetEnumerator();
		}

		#region IList<Cell> Members

		public int IndexOf(Cell item)
		{
			return list.IndexOf(item);
		}

		public void Insert(int index, Cell item)
		{
			list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public Cell this[int index]
		{
			get
			{
				return list[index];
			}
			set
			{
				list[index] = value;
			}
		}

		#endregion
	}
}
